/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2017 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Data
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;

    using Omu.ValueInjecter;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The dynamic db function.
    /// </summary>
    public class DynamicDbFunction : IDbFunctionSession
    {
        #region Fields

        /// <summary>
        ///     The _db access provider.
        /// </summary>
        private readonly IDbAccessProvider _dbAccessProvider;

        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        ///     The _get data proxy.
        /// </summary>
        private readonly TryInvokeMemberProxy _getDataProxy;

        /// <summary>
        ///     The _get data set proxy.
        /// </summary>
        private readonly TryInvokeMemberProxy _getDataSetProxy;

        /// <summary>
        ///     The _get reader proxy.
        /// </summary>
        private readonly TryInvokeMemberProxy _getReaderProxy;

        /// <summary>
        ///     The _query proxy.
        /// </summary>
        private readonly TryInvokeMemberProxy _queryProxy;

        /// <summary>
        ///     The _scalar proxy.
        /// </summary>
        private readonly TryInvokeMemberProxy _scalarProxy;

        /// <summary>
        ///     The _db transaction.
        /// </summary>
        private IDbTransaction _dbTransaction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDbFunction"/> class.
        /// </summary>
        /// <param name="dbAccessProvider">
        /// The db Access Provider. 
        /// </param>
        /// <param name="dbSpecificFunctions">
        /// The db Specific Functions. 
        /// </param>
        /// <param name="dbFilterFunctions">
        /// The db Filter Functions. 
        /// </param>
        public DynamicDbFunction(
            [NotNull] IDbAccessProvider dbAccessProvider, 
            IServiceLocator serviceLocator)
        {
            this._dbAccessProvider = dbAccessProvider;
            this._serviceLocator = serviceLocator;

            this._getDataProxy = new TryInvokeMemberProxy(this.InvokeGetData);
            this._getDataSetProxy = new TryInvokeMemberProxy(this.InvokeGetDataSet);
            this._queryProxy = new TryInvokeMemberProxy(this.InvokeQuery);
            this._scalarProxy = new TryInvokeMemberProxy(this.InvokeScalar);
            this._getReaderProxy = new TryInvokeMemberProxy(this.InvokeDataReader);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets UnitOfWork.
        /// </summary>
        public IDbTransaction DbTransaction
        {
            get
            {
                return this._dbTransaction;
            }

            protected set
            {
                this._dbTransaction = value;
            }
        }

        /// <summary>
        ///     Gets GetData.
        /// </summary>
        public dynamic GetData
        {
            get
            {
                return this._getDataProxy.ToDynamic();
            }
        }

        /// <summary>
        ///     Gets GetDataSet.
        /// </summary>
        public dynamic GetDataSet
        {
            get
            {
                return this._getDataSetProxy.ToDynamic();
            }
        }

        /// <summary>
        ///     Gets the get reader.
        /// </summary>
        public dynamic GetReader
        {
            get
            {
                return this._getReaderProxy.ToDynamic();
            }
        }

        /// <summary>
        ///     Gets Query.
        /// </summary>
        public dynamic Query
        {
            get
            {
                return this._queryProxy.ToDynamic();
            }
        }

        /// <summary>
        ///     Gets Scalar.
        /// </summary>
        public dynamic Scalar
        {
            get
            {
                return this._scalarProxy.ToDynamic();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create session.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level. 
        /// </param>
        /// <returns>
        /// The <see cref="IDbFunctionSession"/> . 
        /// </returns>
        public IDbFunctionSession CreateSession(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return new DynamicDbFunction(this._dbAccessProvider, this._serviceLocator)
                       {
                           DbTransaction = this._dbAccessProvider.Instance.BeginTransaction(isolationLevel)
                       };
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this._dbTransaction == null)
            {
                return;
            }

            if (this._dbTransaction.Connection != null)
            {
                if (this._dbTransaction.Connection.State == ConnectionState.Open)
                {
                    this._dbTransaction.Connection.Close();
                }
            }

            this._dbTransaction.Dispose();
            this._dbTransaction = null;
        }

        /// <summary>
        /// The get typed.
        /// </summary>
        /// <param name="getFunction">
        /// The get function. 
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IList"/> . 
        /// </returns>
        public IList<T> GetTyped<T>(Func<dynamic, object> getFunction)
            where T : new()
        {
            var objectList = new List<T>();

            using (var dataReader = (IDataReader)getFunction(this.GetReader))
            {
                while (dataReader.Read())
                {
                    var o = new T();
                    o.InjectFrom<DataRecordInjection>(dataReader);
                    objectList.Add(o);
                }

                dataReader.Close();
            }

            return objectList;
        }

        #endregion

        #region Methods

        public IEnumerable<IDbSpecificFunction> DbSpecificFunctions
        {
            get
            {
                return this._serviceLocator.Get<IEnumerable<IDbSpecificFunction>>()
                    .WhereProviderName(this._dbAccessProvider.ProviderName)
                    .BySortOrder()
                    .ToList();
            }
        }

        /// <summary>
        /// The db function execute.
        /// </summary>
        /// <param name="functionType">
        /// The function Type. 
        /// </param>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="parameters">
        /// The parameters. 
        /// </param>
        /// <param name="executeDb">
        /// The execute db. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The db function execute. 
        /// </returns>
        protected bool DbFunctionExecute(
            DbFunctionType functionType, 
            [NotNull] InvokeMemberBinder binder, 
            [NotNull] IList<KeyValuePair<string, object>> parameters, 
            [NotNull] Func<IDbCommand, object> executeDb, 
            [CanBeNull] out object result)
        {
            CodeContracts.VerifyNotNull(binder, "binder");
            CodeContracts.VerifyNotNull(parameters, "parameters");
            CodeContracts.VerifyNotNull(executeDb, "executeDb");

            var operationName = binder.Name;

            // see if there's a specific function override for the current provider...
            var specificFunction = this.DbSpecificFunctions
                .WhereOperationSupported(operationName)
                .FirstOrDefault();

            result = null;

            if (specificFunction != null)
            {
                if (!specificFunction.Execute(functionType, operationName, parameters, out result, this.DbTransaction))
                {
                    // unsuccessful -- execute command below
                    specificFunction = null;
                }
            }

            if (specificFunction == null)
            {
                using (var cmd = this._dbAccessProvider.Instance.GetCommand(operationName, CommandType.StoredProcedure, parameters))
                {
                    result = executeDb(cmd);
                }
            }

            this.RunFilters(functionType, parameters, operationName, ref result);
 
            return true;
        }

        /// <summary>
        /// The invoke data reader.
        /// </summary>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> . 
        /// </returns>
        protected bool InvokeDataReader(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (this.DbTransaction == null)
            {
                throw new ArgumentNullException("UnitOfWork", "GetReader must be executed in the context of an Session.");
            }

            return this.DbFunctionExecute(
                DbFunctionType.Reader, 
                binder, 
                this.MapParameters(binder.CallInfo, args), 
                (cmd) => this._dbAccessProvider.Instance.GetReader(cmd, this.DbTransaction), 
                out result);
        }

        /// <summary>
        /// The invoke get data.
        /// </summary>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The invoke get data. 
        /// </returns>
        protected bool InvokeGetData(
            [NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
        {
            return this.DbFunctionExecute(
                DbFunctionType.DataTable, 
                binder, 
                this.MapParameters(binder.CallInfo, args), 
                (cmd) => this._dbAccessProvider.Instance.GetData(cmd, this.DbTransaction), 
                out result);
        }

        /// <summary>
        /// The invoke get data set.
        /// </summary>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The invoke get data set. 
        /// </returns>
        protected bool InvokeGetDataSet(
            [NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
        {
            return this.DbFunctionExecute(
                DbFunctionType.DataSet, 
                binder, 
                this.MapParameters(binder.CallInfo, args), 
                (cmd) => this._dbAccessProvider.Instance.GetDataset(cmd, this.DbTransaction), 
                out result);
        }

        /// <summary>
        /// The invoke query.
        /// </summary>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The invoke query. 
        /// </returns>
        protected bool InvokeQuery([NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
        {
            return this.DbFunctionExecute(
                DbFunctionType.Query, 
                binder, 
                this.MapParameters(binder.CallInfo, args), 
                (cmd) =>
                    {
                        this._dbAccessProvider.Instance.ExecuteNonQuery(cmd, this.DbTransaction);
                        return null;
                    }, 
                out result);
        }

        /// <summary>
        /// The invoke scalar.
        /// </summary>
        /// <param name="binder">
        /// The binder. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The invoke scalar. 
        /// </returns>
        protected bool InvokeScalar([NotNull] InvokeMemberBinder binder, [NotNull] object[] args, [NotNull] out object result)
        {
            return this.DbFunctionExecute(
                DbFunctionType.Scalar, 
                binder, 
                this.MapParameters(binder.CallInfo, args), 
                (cmd) => this._dbAccessProvider.Instance.ExecuteScalar(cmd, this.DbTransaction), 
                out result);
        }

        /// <summary>
        /// The map parameters.
        /// </summary>
        /// <param name="callInfo">
        /// The call info. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <returns>
        /// The <see cref="IList"/> . 
        /// </returns>
        [NotNull]
        protected IList<KeyValuePair<string, object>> MapParameters([NotNull] CallInfo callInfo, [NotNull] object[] args)
        {
            CodeContracts.VerifyNotNull(callInfo, "callInfo");
            CodeContracts.VerifyNotNull(args, "args");

            var argsPairs =
                args.Reverse()
                    .Zip(callInfo.ArgumentNames.Reverse().Infinite(), (a, name) => new KeyValuePair<string, object>(name, a))
                    .Reverse()
                    .ToList();

            var entities = argsPairs.Where(x => x.Value is IEntity).ToList();

            foreach (var pair in entities)
            {
                // remove the individual entity
                argsPairs.Remove(pair);

                // Add all items in this object...
                argsPairs.AddRange(pair.AnyToDictionary());
            }

            return argsPairs;
        }

        /// <summary>
        /// The run filters.
        /// </summary>
        /// <param name="functionType">
        /// The function type.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        private void RunFilters(DbFunctionType functionType, IList<KeyValuePair<string, object>> parameters, string operationName, ref object result)
        {
            // execute filter...
            var filterFunctions = this._serviceLocator.Get<IEnumerable<IDbDataFilter>>()
                .BySortOrder()
                .WhereOperationSupported(operationName)
                .ToList();

            foreach (var filter in filterFunctions)
            {
                filter.Run(functionType, operationName, parameters, result);
            }
        }

        #endregion
    }
}