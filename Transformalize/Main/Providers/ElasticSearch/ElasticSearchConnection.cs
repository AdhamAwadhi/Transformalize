using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.Elasticsearch.Net.Domain;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchConnection : AbstractConnection {

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return string.Empty; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }

        public ElasticSearchConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.ElasticSearch;
            IsDatabase = true;
        }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }

            var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(processName));
            var result = client.Client.SearchGet(client.Index, client.Type, s => (s
                .Add("q", string.Format("process:{0}", processName))
                .Add("_source", false)
                .Add("fields", "tflbatchid")
            ));

            //this just can't be how you're suppose to do it...
            var max = 1;
            var hits = result.Response["hits"].hits;
            for (var i = 0; i < result.Response["hits"].total; i++) {
                var value = (int) hits[i]["fields"]["tflbatchid"].Value[0];
                if (value > max)
                    max = value;
            }
            return max+1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
            var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(entity.ProcessName));
            var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
            var body = new {
                id = entity.TflBatchId,
                tflbatchid = entity.TflBatchId,
                process = entity.ProcessName,
                connection = input.Name,
                entity = entity.Alias,
                updates = entity.Updates,
                inserts = entity.Inserts,
                deletes = entity.Deletes,
                version = new DefaultFactory().Convert(entity.End, versionType),
                version_type = versionType,
                tflupdate = DateTime.UtcNow
            };
            client.Client.Index(client.Index, client.Type, body);
        }
    }
}