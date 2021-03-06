#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Providers.Ado;
using Transformalize.Providers.PostgreSql;
using Transformalize.Transforms.System;

namespace Transformalize.Ioc.Autofac.Modules {
    public class AdoModule : Module {
        private readonly Process _process;
        private readonly HashSet<string> _ado = new HashSet<string>(Constants.AdoProviderSet());

        public AdoModule() { }

        public AdoModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // for now, these have been moved to plug-ins
            _ado.Remove("access");
            _ado.Remove("sqlserver");
            _ado.Remove("mysql");
            _ado.Remove("sqlite");
            _ado.Remove("sqlce");

            // connections
            foreach (var connection in _process.Connections.Where(c => _ado.Contains(c.Provider))) {

                // Connection Factory
                builder.Register<IConnectionFactory>(ctx => {
                    switch (connection.Provider) {
                        case "postgresql":
                            return new PostgreSqlConnectionFactory(connection);
                        default:
                            return new NullConnectionFactory();
                    }
                }).Named<IConnectionFactory>(connection.Key).InstancePerLifetimeScope();

                // Schema Reader
                builder.Register<ISchemaReader>(ctx => {
                    var factory = ctx.ResolveNamed<IConnectionFactory>(connection.Key);
                    return new AdoSchemaReader(ctx.ResolveNamed<IConnectionContext>(connection.Key), factory);
                }).Named<ISchemaReader>(connection.Key);

            }

            //ISchemaReader
            //IOutputController
            //IRead (Process for Calculated Columns)
            //IWrite (Process for Calculated Columns)
            //IInitializer (Process)

            // Per Entity
            // IInputVersionDetector
            // IRead (Input, per Entity)
            // IOutputController
            // -- IBatchReader (for matching)
            // -- IWriteMasterUpdateQuery (for updating)
            // IUpdate
            // IWrite
            // IEntityDeleteHandler

            // entitiy input
            foreach (var entity in _process.Entities.Where(e => _ado.Contains(_process.Connections.First(c => c.Name == e.Connection).Provider))) {

                // INPUT READER
                builder.Register<IRead>(ctx => {

                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                    var dataReader = new AdoInputReader(
                        input,
                        input.InputFields,
                        ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key),
                        rowFactory
                    );

                    return dataReader;

                }).Named<IRead>(entity.Key);

                // INPUT VERSION DETECTOR
                builder.Register<IInputProvider>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    switch (input.Connection.Provider) {
                        case "postgresql":
                            return new AdoInputProvider(input, ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key));
                        default:
                            return new NullInputProvider();
                    }
                }).Named<IInputProvider>(entity.Key);

            }

            // entity output
            if (_ado.Contains(_process.Output().Provider)) {

                var calc = _process.ToCalculatedFieldsProcess();

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => {
                    var output = ctx.Resolve<OutputContext>();
                    if (_process.Mode != "init")
                        return new NullOutputController();

                    switch (output.Connection.Provider) {
                        case "postgresql":
                            var actions = new List<IAction> { new AdoStarViewCreator(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)) };
                            if (_process.Flatten) {
                                actions.Add(new AdoFlatTableCreator(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)));
                            }
                            return new AdoStarController(output, actions);
                        default:
                            return new NullOutputController();
                    }
                }).As<IOutputController>();

                // PROCESS CALCULATED READER
                builder.Register<IRead>(ctx => {
                    var calcContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, calc.Entities.First());
                    var outputContext = new OutputContext(calcContext);
                    var cf = ctx.ResolveNamed<IConnectionFactory>(outputContext.Connection.Key);
                    var capacity = outputContext.Entity.Fields.Count + outputContext.Entity.CalculatedFields.Count;
                    var rowFactory = new RowFactory(capacity, false, false);
                    return new AdoStarParametersReader(outputContext, _process, cf, rowFactory);
                }).As<IRead>();

                // PROCESS CALCULATED FIELD WRITER
                builder.Register<IWrite>(ctx => {
                    var calcContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, calc.Entities.First());
                    var outputContext = new OutputContext(calcContext);
                    var cf = ctx.ResolveNamed<IConnectionFactory>(outputContext.Connection.Key);
                    return new AdoCalculatedFieldUpdater(outputContext, _process, cf);
                }).As<IWrite>();

                // PROCESS INITIALIZER
                builder.Register<IInitializer>(ctx => {
                    var output = ctx.Resolve<OutputContext>();
                    return new AdoInitializer(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key));
                }).As<IInitializer>();

                // ENTITIES
                foreach (var entity in _process.Entities) {

                    builder.Register<IOutputProvider>(ctx => {

                        IWrite writer;
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var cf = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                        var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", output.GetAllEntityFields().Count()));

                        // matcher determines what's an update vs. and insert
                        var matcher = entity.Update ? (IBatchReader)new AdoEntityMatchingKeysReader(output, cf, rowFactory) : new NullBatchReader();

                        switch (output.Connection.Provider) {
                            case "postgresql":
                                writer = new AdoEntityWriter(
                                    output,
                                    matcher,
                                    new AdoEntityInserter(output, cf),
                                    entity.Update ? (IWrite)new AdoEntityUpdater(output, cf) : new NullWriter(output)
                                );
                                break;
                            default:
                                writer = new NullWriter(output);
                                break;
                        }

                        return new AdoOutputProvider(output, cf, writer);
                    }).Named<IOutputProvider>(entity.Key);

                    // ENTITY OUTPUT CONTROLLER
                    builder.Register<IOutputController>(ctx => {

                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var initializer = _process.Mode == "init" ? (IAction)new AdoEntityInitializer(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)) : new NullInitializer();

                        switch (output.Connection.Provider) {
                            case "postgresql":
                                return new AdoOutputController(
                                    output,
                                    initializer,
                                    ctx.ResolveNamed<IInputProvider>(entity.Key),
                                    ctx.ResolveNamed<IOutputProvider>(entity.Key),
                                    ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)
                                );
                            default:
                                return new NullOutputController();
                        }

                    }).Named<IOutputController>(entity.Key);

                    // MASTER UPDATE QUERY
                    builder.Register<IWriteMasterUpdateQuery>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var factory = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                        switch (output.Connection.Provider) {
                            case "postgresql":
                                return new PostgreSqlUpdateMasterKeysQueryWriter(output, factory);
                            default:
                                return new AdoUpdateMasterKeysQueryWriter(output, factory);
                        }
                    }).Named<IWriteMasterUpdateQuery>(entity.Key + "MasterKeys");

                    // MASTER UPDATER
                    builder.Register<IUpdate>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        switch (output.Connection.Provider) {
                            case "postgresql":
                                return new AdoMasterUpdater(
                                    output,
                                    ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key),
                                    ctx.ResolveNamed<IWriteMasterUpdateQuery>(entity.Key + "MasterKeys")
                                );
                            default:
                                return new NullMasterUpdater();
                        }
                    }).Named<IUpdate>(entity.Key);

                    // DELETE HANDLER
                    if (entity.Delete) {

                        // register input keys and hashcode reader if necessary
                        builder.Register(ctx => {
                            var inputContext = ctx.ResolveNamed<InputContext>(entity.Key);
                            var rowCapacity = inputContext.Entity.GetPrimaryKey().Count();
                            var rowFactory = new RowFactory(rowCapacity, false, true);

                            switch (inputContext.Connection.Provider) {
                                case "postgresql":
                                    return new AdoReader(
                                        inputContext,
                                        entity.GetPrimaryKey(),
                                        ctx.ResolveNamed<IConnectionFactory>(inputContext.Connection.Key),
                                        rowFactory,
                                        ReadFrom.Input
                                    );
                                default:
                                    return ctx.IsRegisteredWithName<IReadInputKeysAndHashCodes>(entity.Key) ? ctx.ResolveNamed<IReadInputKeysAndHashCodes>(entity.Key) : new NullReader(inputContext);
                            }
                        }).Named<IReadInputKeysAndHashCodes>(entity.Key);

                        // register output keys and hash code reader if necessary
                        builder.Register((ctx => {
                            var context = ctx.ResolveNamed<OutputContext>(entity.Key);
                            var rowCapacity = context.Entity.GetPrimaryKey().Count();
                            var rowFactory = new RowFactory(rowCapacity, false, true);

                            var outputConnection = _process.Output();
                            switch (outputConnection.Provider) {
                                case "postgresql":
                                    var ocf = ctx.ResolveNamed<IConnectionFactory>(outputConnection.Key);
                                    return new AdoReader(context, entity.GetPrimaryKey(), ocf, rowFactory, ReadFrom.Output);
                                default:
                                    return ctx.IsRegisteredWithName<IReadOutputKeysAndHashCodes>(entity.Key) ? ctx.ResolveNamed<IReadOutputKeysAndHashCodes>(entity.Key) : new NullReader(context);
                            }

                        })).Named<IReadOutputKeysAndHashCodes>(entity.Key);

                        builder.Register((ctx) => {
                            var outputConnection = _process.Output();
                            var outputContext = ctx.ResolveNamed<OutputContext>(entity.Key);

                            switch (outputConnection.Provider) {
                                case "postgresql":
                                    var ocf = ctx.ResolveNamed<IConnectionFactory>(outputConnection.Key);
                                    return new AdoDeleter(outputContext, ocf);
                                default:
                                    return ctx.IsRegisteredWithName<IDelete>(entity.Key) ? ctx.ResolveNamed<IDelete>(entity.Key) : new NullDeleter(outputContext);
                            }
                        }).Named<IDelete>(entity.Key);

                        builder.Register<IEntityDeleteHandler>(ctx => {
                            var context = ctx.ResolveNamed<IContext>(entity.Key);
                            var primaryKey = entity.GetPrimaryKey();

                            var handler = new DefaultDeleteHandler(
                                context,
                                ctx.ResolveNamed<IReadInputKeysAndHashCodes>(entity.Key),
                                ctx.ResolveNamed<IReadOutputKeysAndHashCodes>(entity.Key),
                                ctx.ResolveNamed<IDelete>(entity.Key)
                            );

                            // since the primary keys from the input may have been transformed into the output, you have to transform before comparing
                            // feels a lot like entity pipeline on just the primary keys... may look at consolidating
                            handler.Register(new DefaultTransform(context, entity.GetPrimaryKey().ToArray()));
                            handler.Register(TransformFactory.GetTransforms(ctx, context, primaryKey));
                            handler.Register(new StringTruncateTransfom(context, primaryKey));

                            return new ParallelDeleteHandler(handler);
                        }).Named<IEntityDeleteHandler>(entity.Key);
                    }


                }
            }
        }

    }
}