﻿using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.DSL.Facets
{
	public class DateExpressionRange 
	{
		[JsonProperty(PropertyName = "from")]
		internal string _From { get; set; }

		[JsonProperty(PropertyName = "to")]
		internal string _To { get; set; }

		public DateExpressionRange From(string value)
		{
			this._From = value;
			return this;
		}
		public DateExpressionRange To(string value)
		{
			this._To = value;
			return this;
		}
	}
}