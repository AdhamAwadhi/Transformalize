﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Elasticsearch.Net.Connection
{
	/// <summary>
	/// 
	/// </summary>
	public class InMemoryConnection : HttpConnection
	{
		private ElasticsearchResponse _fixedResult;
		private readonly byte[] _fixedResultBytes = Encoding.UTF8.GetBytes("{ \"USING NEST IN MEMORY CONNECTION\"  : null }");

		public InMemoryConnection(IConnectionConfigurationValues settings)
			: base(settings)
		{

		}
		public InMemoryConnection(IConnectionConfigurationValues settings, ElasticsearchResponse fixedResult)
			: base(settings)
		{
			this._fixedResult = fixedResult;
		}

		protected override ElasticsearchResponse DoSynchronousRequest(HttpWebRequest request, byte[] data = null)
		{
			return this.ReturnConnectionStatus(request, data);
		}

		private ElasticsearchResponse ReturnConnectionStatus(HttpWebRequest request, byte[] data)
		{
			var method = request.Method;
			var path = request.RequestUri.ToString();

			var cs = ElasticsearchResponse.Create(this._ConnectionSettings, 200, method, path, data, _fixedResultBytes);
			_ConnectionSettings.ConnectionStatusHandler(cs);
			return cs;
		}

		protected override Task<ElasticsearchResponse> DoAsyncRequest(HttpWebRequest request, byte[] data = null)
		{
			return Task.Factory.StartNew<ElasticsearchResponse>(() =>
			{
				var cs = this.ReturnConnectionStatus(request, data);
				_ConnectionSettings.ConnectionStatusHandler(cs);
				return cs;
			});
		}

	}
}
