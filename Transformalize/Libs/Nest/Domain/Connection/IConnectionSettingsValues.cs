﻿using System;
using System.Collections.ObjectModel;
using Elasticsearch.Net.Connection;
using Newtonsoft.Json;

namespace Nest
{
	public interface IConnectionSettingsValues : IConnectionConfigurationValues
	{
		ElasticInferrer Inferrer { get; }
		FluentDictionary<Type, string> DefaultIndices { get; }
		FluentDictionary<Type, string> DefaultTypeNames { get; }
		string DefaultIndex { get; }
		Func<string, string> DefaultPropertyNameInferrer { get; }
		Func<Type, string> DefaultTypeNameInferrer { get; }
		Action<JsonSerializerSettings> ModifyJsonSerializerSettings { get; }
		ReadOnlyCollection<Func<Type, JsonConverter>> ContractConverters { get; }
	}
}