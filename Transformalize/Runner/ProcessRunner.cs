#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Processes;

namespace Transformalize.Runner {
    public class ProcessRunner : IProcessRunner {

        private AbstractPipelineExecuter _pipelineExecuter = new ThreadPoolPipelineExecuter();

        public IEnumerable<IEnumerable<Row>> Run(Process process) {
            var results = new List<IEnumerable<Row>>();

            if (!process.IsReady())
                return results;

            if (process.Options.Mode == "test")
                _pipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            ProcessDeletes(process);
            ProcessEntities(process);
            ProcessMaster(process);
            ProcessTransforms(process);

            if (process.Options.RenderTemplates)
                new TemplateManager(process).Manage();

            return process.Entities.Select(e => e.Rows);
        }

        private void ProcessDeletes(Process process) {
            foreach (var entityDeleteProcess in process.Entities.Where(e => e.Delete).Select(entity => new EntityDeleteProcess(process, entity))) {
                entityDeleteProcess.PipelineExecuter = _pipelineExecuter;
                entityDeleteProcess.Execute();
            }
        }

        private void ProcessEntities(Process process) {

            foreach (var entityKeysProcess in process.Entities.Where(e => e.InputConnection.Provider.IsDatabase).Select(entity => new EntityKeysProcess(process, entity))) {
                entityKeysProcess.PipelineExecuter = _pipelineExecuter;
                entityKeysProcess.Execute();
            }

            foreach (var entityProcess in process.Entities.Select(entity => new EntityProcess(process, entity))) {
                entityProcess.PipelineExecuter = _pipelineExecuter;
                entityProcess.Execute();
            }
        }

        private void ProcessMaster(Process process) {
            if (process.OutputConnection.Provider.Type == ProviderType.Internal)
                return;
            var updateMasterProcess = new UpdateMasterProcess(ref process) { PipelineExecuter = _pipelineExecuter };
            updateMasterProcess.Execute();
        }

        private void ProcessTransforms(Process process) {
            if (process.CalculatedFields.Count <= 0)
                return;
            if (process.OutputConnection.Provider.Type == ProviderType.Internal)
                return;
            var transformProcess = new TransformProcess(process) { PipelineExecuter = _pipelineExecuter };
            transformProcess.Execute();
        }

        public void Dispose() {
            LogManager.Flush();
        }

    }
}