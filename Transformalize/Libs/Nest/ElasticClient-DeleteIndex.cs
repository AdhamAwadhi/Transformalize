﻿using System;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;

namespace Transformalize.Libs.Nest
{
	public partial class ElasticClient
	{

		/// <inheritdoc />
		public IIndicesResponse DeleteIndex(Func<DeleteIndexDescriptor, DeleteIndexDescriptor> selector)
		{
			return this.Dispatch<DeleteIndexDescriptor, DeleteIndexRequestParameters, IndicesResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesDeleteDispatch<IndicesResponse>(p)
			);
		}

		/// <inheritdoc />
		public IIndicesResponse DeleteIndex(IDeleteIndexRequest deleteIndexRequest)
		{
			return this.Dispatch<IDeleteIndexRequest, DeleteIndexRequestParameters, IndicesResponse>(
				deleteIndexRequest,
				(p, d) => this.RawDispatch.IndicesDeleteDispatch<IndicesResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<IIndicesResponse> DeleteIndexAsync(Func<DeleteIndexDescriptor, DeleteIndexDescriptor> selector)
		{
			return this.DispatchAsync<DeleteIndexDescriptor, DeleteIndexRequestParameters, IndicesResponse, IIndicesResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesDeleteDispatchAsync<IndicesResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<IIndicesResponse> DeleteIndexAsync(IDeleteIndexRequest deleteIndexRequest)
		{
			return this.DispatchAsync<IDeleteIndexRequest, DeleteIndexRequestParameters, IndicesResponse, IIndicesResponse>(
				deleteIndexRequest,
				(p, d) => this.RawDispatch.IndicesDeleteDispatchAsync<IndicesResponse>(p)
			);
		}

	}
}