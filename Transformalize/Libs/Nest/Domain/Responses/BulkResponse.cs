﻿using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	[JsonObject]
	public class BulkResponse : BaseResponse, IBulkResponse
	{
		private bool _isValid;
		public override bool IsValid
		{
			get
			{
				return this._isValid && !this.Errors && !this.ItemsWithErrors.HasAny();
			}
			internal set
			{
				this._isValid = value;
			}
		}

		[JsonProperty("took")]
		public int Took { get; internal set; }

		[JsonProperty("errors")]
		public bool Errors { get; internal set; }

		[JsonProperty("items")]
		public IEnumerable<BulkOperationResponseItem> Items { get; internal set; }

		[JsonIgnore]
		public IEnumerable<BulkOperationResponseItem> ItemsWithErrors
		{
			get
			{
				return !this.Items.HasAny() ? Enumerable.Empty<BulkOperationResponseItem>() : this.Items.Where(i => !i.IsValid);
			}
		}
	}
}