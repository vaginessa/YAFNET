﻿using YAF.Lucene.Net.QueryParsers.Classic;
using YAF.Lucene.Net.Search;

namespace YAF.Lucene.Net.QueryParsers.Ext
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     https://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// This class represents an extension base class to the Lucene standard
    /// <see cref="Classic.QueryParser"/>. The
    /// <see cref="Classic.QueryParser"/> is generated by the JavaCC
    /// parser generator. Changing or adding functionality or syntax in the standard
    /// query parser requires changes to the JavaCC source file. To enable extending
    /// the standard query parser without changing the JavaCC sources and re-generate
    /// the parser the <see cref="ParserExtension"/> can be customized and plugged into an
    /// instance of <see cref="ExtendableQueryParser"/>, a direct subclass of
    /// <see cref="Classic.QueryParser"/>.
    /// </summary>
    /// <seealso cref="Extensions"/>
    /// <seealso cref="ExtendableQueryParser"/>
    public abstract class ParserExtension
    {
        /// <summary>
        /// Processes the given <see cref="ExtensionQuery"/> and returns a corresponding
        /// <see cref="Query"/> instance. Subclasses must either return a <see cref="Query"/>
        /// instance or raise a <see cref="ParseException"/>. This method must not return
        /// <c>null</c>.
        /// </summary>
        /// <param name="query">the extension query</param>
        /// <returns>a new query instance</returns>
        /// <exception cref="ParseException">if the query can not be parsed.</exception>
        public abstract Query Parse(ExtensionQuery query);
    }
}
