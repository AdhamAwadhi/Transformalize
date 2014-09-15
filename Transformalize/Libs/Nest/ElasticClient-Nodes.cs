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
		public INodeInfoResponse NodesInfo(Func<NodesInfoDescriptor, NodesInfoDescriptor> selector = null)
		{
			selector = selector ?? (s => s);
			return this.Dispatch<NodesInfoDescriptor, NodesInfoRequestParameters, NodeInfoResponse>(
				selector,
				(p, d) => this.RawDispatch.NodesInfoDispatch<NodeInfoResponse>(p)
			);
		}

		/// <inheritdoc />
		public INodeInfoResponse NodesInfo(INodesInfoRequest nodesInfoRequest)
		{
			return this.Dispatch<INodesInfoRequest, NodesInfoRequestParameters, NodeInfoResponse>(
				nodesInfoRequest,
				(p, d) => this.RawDispatch.NodesInfoDispatch<NodeInfoResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<INodeInfoResponse> NodesInfoAsync(Func<NodesInfoDescriptor, NodesInfoDescriptor> selector = null)
		{
			selector = selector ?? (s => s);
			return this.DispatchAsync<NodesInfoDescriptor, NodesInfoRequestParameters, NodeInfoResponse, INodeInfoResponse>(
				selector,
				(p, d) => this.RawDispatch.NodesInfoDispatchAsync<NodeInfoResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<INodeInfoResponse> NodesInfoAsync(INodesInfoRequest nodesInfoRequest)
		{
			return this.DispatchAsync<INodesInfoRequest, NodesInfoRequestParameters, NodeInfoResponse, INodeInfoResponse>(
				nodesInfoRequest,
				(p, d) => this.RawDispatch.NodesInfoDispatchAsync<NodeInfoResponse>(p)
			);
		}




		/// <inheritdoc />
		public INodeStatsResponse NodesStats(Func<NodesStatsDescriptor, NodesStatsDescriptor> selector = null)
		{
			selector = selector ?? (s => s);
			return this.Dispatch<NodesStatsDescriptor, NodesStatsRequestParameters, NodeStatsResponse>(
				selector,
				(p, d) => this.RawDispatch.NodesStatsDispatch<NodeStatsResponse>(p)
			);
		}

		/// <inheritdoc />
		public INodeStatsResponse NodesStats(INodesStatsRequest nodesStatsRequest)
		{
			return this.Dispatch<INodesStatsRequest, NodesStatsRequestParameters, NodeStatsResponse>(
				nodesStatsRequest,
				(p, d) => this.RawDispatch.NodesStatsDispatch<NodeStatsResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<INodeStatsResponse> NodesStatsAsync(Func<NodesStatsDescriptor, NodesStatsDescriptor> selector = null)
		{
			selector = selector ?? (s => s);
			return this.DispatchAsync<NodesStatsDescriptor, NodesStatsRequestParameters, NodeStatsResponse, INodeStatsResponse>(
				selector,
				(p, d) => this.RawDispatch.NodesStatsDispatchAsync<NodeStatsResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<INodeStatsResponse> NodesStatsAsync(INodesStatsRequest nodesStatsRequest)
		{
			return this.DispatchAsync<INodesStatsRequest, NodesStatsRequestParameters, NodeStatsResponse, INodeStatsResponse>(
				nodesStatsRequest,
				(p, d) => this.RawDispatch.NodesStatsDispatchAsync<NodeStatsResponse>(p)
			);
		}
	}
}