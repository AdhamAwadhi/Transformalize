﻿using System;
using System.Diagnostics;
using Transformalize.Libs.Elasticsearch.Net.Domain;

namespace Transformalize.Libs.Elasticsearch.Net.Connection
{
	public class ElasticsearchResponseTracer : IDisposable
	{
		private readonly bool _enabled;
		private Stopwatch _stopwatch;

		public ElasticsearchResponse _result { get; set; }

		public ElasticsearchResponseTracer(bool enabled)
		{
			this._enabled = enabled;
			if (enabled)
			{
				this._stopwatch = Stopwatch.StartNew();
			}
		}

		public void SetResult(ElasticsearchResponse status)
		{
			if (!_enabled)
				return;
			this._result = status;
			this._stopwatch.Stop();
		}

		public void Dispose()
		{
			if (!_enabled || this._result == null)
				return;

			if (_result.Success)
			{
				Trace.TraceInformation(
					"NEST {0} {1} ({2}):\r\n{3}"
					, _result.RequestMethod
					, _result.RequestUrl
					, _stopwatch.Elapsed.ToString()
					, _result.ToString()
				);
			}
			else
			{
				Trace.TraceError(
					"NEST {0} {1} ({2}):\r\n{3}"
					, _result.RequestMethod
					, _result.RequestUrl
					, _stopwatch.Elapsed.ToString()
					, _result.ToString()
				);
			}
		}

	}
}
