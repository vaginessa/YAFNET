﻿// ***********************************************************************
// <copyright file="EachScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Handlebars.js like each block
/// Usages: {{#each customers}} {{Name}} {{/each}}
/// {{#each customers}} {{it.Name}} {{/each}}
/// {{#each num in numbers}} {{num}} {{/each}}
/// {{#each num in [1,2,3]}} {{num}} {{/each}}
/// {{#each numbers}} {{it}} {{else}} no numbers {{/each}}
/// {{#each numbers}} {{it}} {{else if letters != null}} has letters {{else}} no numbers {{/each}}
/// {{#each n in numbers where n &gt; 5}} {{it}} {{else}} no numbers &gt; 5 {{/each}}
/// {{#each n in numbers where n &gt; 5 orderby n skip 1 take 2}} {{it}} {{else}} no numbers &gt; 5 {{/each}}
/// </summary>
public class EachScriptBlock : ScriptBlock
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name => "each";

    /// <summary>
    /// Write as an asynchronous operation.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="System.NotSupportedException">'each' block requires the collection to iterate</exception>
    public async override Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
    {
        if (block.Argument.IsNullOrEmpty())
            throw new NotSupportedException("'each' block requires the collection to iterate");

        var cache = (EachArg)scope.Context.Cache.GetOrAdd(block.ArgumentString, _ => ParseArgument(scope, block));

        var collection = cache.Source.Evaluate(scope, out var syncResult, out var asyncResult)
                             ? (IEnumerable)syncResult
                             : (IEnumerable)await asyncResult.ConfigAwait();

        var index = 0;
        if (collection != null)
        {
            if (cache.Where != null || cache.OrderBy != null || cache.OrderByDescending != null ||
                cache.Skip != null || cache.Take != null)
            {
                var filteredResults = new List<Dictionary<string, object>>();
                foreach (var element in collection)
                {
                    // Add all properties into scope if called without explicit in argument
                    var scopeArgs = !cache.HasExplicitBinding && CanExportScopeArgs(element)
                                        ? element.ToObjectDictionary()
                                        : new Dictionary<string, object>();

                    scopeArgs[cache.Binding] = element;
                    scopeArgs[nameof(index)] = AssertWithinMaxQuota(index++);
                    var itemScope = scope.ScopeWithParams(scopeArgs);

                    if (cache.Where != null)
                    {
                        var result = await cache.Where.EvaluateToBoolAsync(itemScope).ConfigAwait();
                        if (!result)
                            continue;
                    }

                    filteredResults.Add(scopeArgs);
                }

                IEnumerable<Dictionary<string, object>> selectedResults = filteredResults;

                var comparer = (IComparer<object>)Comparer<object>.Default;
                if (cache.OrderBy != null)
                {
                    var i = 0;
                    selectedResults = selectedResults.OrderBy(scopeArgs =>
                        {
                            scopeArgs[nameof(index)] = i++;
                            return cache.OrderBy.Evaluate(scope.ScopeWithParams(scopeArgs));
                        }, comparer);
                }
                else if (cache.OrderByDescending != null)
                {
                    var i = 0;
                    selectedResults = selectedResults.OrderByDescending(scopeArgs =>
                        {
                            scopeArgs[nameof(index)] = i++;
                            return cache.OrderByDescending.Evaluate(scope.ScopeWithParams(scopeArgs));
                        }, comparer);
                }

                if (cache.Skip != null)
                {
                    var skip = cache.Skip.Evaluate(scope).ConvertTo<int>();
                    selectedResults = selectedResults.Skip(skip);
                }

                if (cache.Take != null)
                {
                    var take = cache.Take.Evaluate(scope).ConvertTo<int>();
                    selectedResults = selectedResults.Take(take);
                }

                index = 0;
                foreach (var scopeArgs in selectedResults)
                {
                    var itemScope = scope.ScopeWithParams(scopeArgs);
                    itemScope.ScopedParams[nameof(index)] = index++;
                    await WriteBodyAsync(itemScope, block, token).ConfigAwait();
                }
            }
            else
            {
                foreach (var element in collection)
                {
                    // Add all properties into scope if called without explicit in argument
                    var scopeArgs = !cache.HasExplicitBinding && CanExportScopeArgs(element)
                                        ? element.ToObjectDictionary()
                                        : new Dictionary<string, object>();

                    scopeArgs[cache.Binding] = element;
                    scopeArgs[nameof(index)] = AssertWithinMaxQuota(index++);
                    var itemScope = scope.ScopeWithParams(scopeArgs);

                    await WriteBodyAsync(itemScope, block, token).ConfigAwait();
                }
            }
        }

        if (index == 0)
        {
            await WriteElseAsync(scope, block.ElseBlocks, token).ConfigAwait();
        }
    }

    /// <summary>
    /// Parses the argument.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="fragment">The fragment.</param>
    /// <returns>EachArg.</returns>
    /// <exception cref="System.NotSupportedException">'each' block requires the collection to iterate</exception>
    /// <exception cref="System.NotSupportedException">'each' block expected identifier but was {token.DebugToken()}</exception>
    EachArg ParseArgument(ScriptScopeContext scope, PageBlockFragment fragment)
    {
        var literal = fragment.Argument.Span.ParseJsExpression(out var token);
        if (token == null)
            throw new NotSupportedException("'each' block requires the collection to iterate");

        var binding = "it";

        literal = literal.AdvancePastWhitespace();

        JsToken source, orderBy, orderByDescending, skip, take;
        JsToken where = orderBy = orderByDescending = skip = take = null;

        var hasExplicitBinding = literal.StartsWith("in ");
        if (hasExplicitBinding)
        {
            if (token is not JsIdentifier identifier)
                throw new NotSupportedException($"'each' block expected identifier but was {token.DebugToken()}");

            binding = identifier.Name;

            literal = literal.Advance(3);
            literal = literal.ParseJsExpression(out source);
            if (source == null)
                throw new NotSupportedException("'each' block requires the collection to iterate");
        }
        else
        {
            source = token;
        }

        if (literal.StartsWith("where "))
        {
            literal = literal.Advance("where ".Length);
            literal = literal.ParseJsExpression(out where);
        }

        literal = literal.AdvancePastWhitespace();
        if (literal.StartsWith("orderby "))
        {
            literal = literal.Advance("orderby ".Length);
            literal = literal.ParseJsExpression(out orderBy);

            literal = literal.AdvancePastWhitespace();
            if (literal.StartsWith("descending"))
            {
                literal = literal.Advance("descending".Length);
                orderByDescending = orderBy;
                orderBy = null;
            }
        }

        literal = literal.AdvancePastWhitespace();
        if (literal.StartsWith("skip "))
        {
            literal = literal.Advance("skip ".Length);
            literal = literal.ParseJsExpression(out skip);
        }

        literal = literal.AdvancePastWhitespace();
        if (literal.StartsWith("take "))
        {
            literal = literal.Advance("take ".Length);
            literal = literal.ParseJsExpression(out take);
        }

        return new EachArg(binding, hasExplicitBinding, source, where, orderBy, orderByDescending, skip, take);
    }

    /// <summary>
    /// Class EachArg.
    /// </summary>
    class EachArg
    {
        /// <summary>
        /// The binding
        /// </summary>
        public readonly string Binding;
        /// <summary>
        /// The has explicit binding
        /// </summary>
        public readonly bool HasExplicitBinding;
        /// <summary>
        /// The source
        /// </summary>
        public readonly JsToken Source;
        /// <summary>
        /// The where
        /// </summary>
        public readonly JsToken Where;
        /// <summary>
        /// The order by
        /// </summary>
        public readonly JsToken OrderBy;
        /// <summary>
        /// The order by descending
        /// </summary>
        public readonly JsToken OrderByDescending;
        /// <summary>
        /// The skip
        /// </summary>
        public readonly JsToken Skip;
        /// <summary>
        /// The take
        /// </summary>
        public readonly JsToken Take;

        /// <summary>
        /// Initializes a new instance of the <see cref="EachArg" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="hasExplicitBinding">if set to <c>true</c> [has explicit binding].</param>
        /// <param name="source">The source.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="orderByDescending">The order by descending.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        public EachArg(string binding, bool hasExplicitBinding, JsToken source, JsToken where,
                       JsToken orderBy, JsToken orderByDescending, JsToken skip, JsToken take)
        {
            Binding = binding;
            HasExplicitBinding = hasExplicitBinding;
            Source = source;
            Where = where;
            OrderBy = orderBy;
            OrderByDescending = orderByDescending;
            Skip = skip;
            Take = take;
        }
    }
}