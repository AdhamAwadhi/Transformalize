﻿namespace Transformalize.Libs.Nest.Domain.Aggregations
{
	public class RangeItem : BucketAggregationBase, IBucketItem
	{
		public string Key { get; set; }
		public double? From { get; set; }
		public string FromAsString { get; set; }
		public double? To { get; set; }
		public string ToAsString { get; set; }
		public long DocCount { get; set; }
	}
}