﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain;

namespace Transformalize.Libs.Elasticsearch.Net.Connection
{
	public interface IConnection
	{
		Task<ElasticsearchResponse> Get(Uri uri);
		ElasticsearchResponse GetSync(Uri uri);

		Task<ElasticsearchResponse> Head(Uri uri);
		ElasticsearchResponse HeadSync(Uri uri);

		Task<ElasticsearchResponse> Post(Uri uri, byte[] data);
		ElasticsearchResponse PostSync(Uri uri, byte[] data);

		Task<ElasticsearchResponse> Put(Uri uri, byte[] data);
		ElasticsearchResponse PutSync(Uri uri, byte[] data);

		Task<ElasticsearchResponse> Delete(Uri uri);
		ElasticsearchResponse DeleteSync(Uri uri);

		Task<ElasticsearchResponse> Delete(Uri uri, byte[] data);
		ElasticsearchResponse DeleteSync(Uri uri, byte[] data);

		bool Ping(Uri uri);
		IList<Uri> Sniff(Uri uri);

	}
}
