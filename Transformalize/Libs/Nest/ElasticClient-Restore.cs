﻿using System;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest
{
	public partial class ElasticClient
	{
		/// <inheritdoc />
		public IRestoreResponse Restore(IRestoreRequest restoreRequest)
		{
			return this.Dispatch<IRestoreRequest, RestoreRequestParameters, RestoreResponse>(
				restoreRequest,
				(p, d) => this.RawDispatch.SnapshotRestoreDispatch<RestoreResponse>(p, d)
			);
		}

		/// <inheritdoc />
		public IRestoreResponse Restore(string repository, string snapshotName, Func<RestoreDescriptor, RestoreDescriptor> selector = null)
		{
			snapshotName.ThrowIfNullOrEmpty("name");
			repository.ThrowIfNullOrEmpty("repository");
			selector = selector ?? (s => s);
			return this.Dispatch<RestoreDescriptor, RestoreRequestParameters, RestoreResponse>(
				s => selector(s.Snapshot(snapshotName).Repository(repository)),
				(p, d) => this.RawDispatch.SnapshotRestoreDispatch<RestoreResponse>(p, d)
			);
		}
		
		/// <inheritdoc />
		public Task<IRestoreResponse> RestoreAsync(IRestoreRequest restoreRequest)
		{
			return this.DispatchAsync<IRestoreRequest, RestoreRequestParameters, RestoreResponse, IRestoreResponse>(
				restoreRequest,
				(p, d) => this.RawDispatch.SnapshotRestoreDispatchAsync<RestoreResponse>(p, d)
			);
		}

		/// <inheritdoc />
		public Task<IRestoreResponse> RestoreAsync(string repository, string snapshotName, Func<RestoreDescriptor, RestoreDescriptor> selector = null)
		{
			snapshotName.ThrowIfNullOrEmpty("name");
			repository.ThrowIfNullOrEmpty("repository");
			selector = selector ?? (s => s);
			return this.DispatchAsync<RestoreDescriptor, RestoreRequestParameters, RestoreResponse, IRestoreResponse>(
				s => selector(s.Snapshot(snapshotName).Repository(repository)),
				(p, d) => this.RawDispatch.SnapshotRestoreDispatchAsync<RestoreResponse>(p, d)
			);
		}
	}
}