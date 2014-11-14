﻿using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.TermVector
{
    [JsonObject]
    public class Token
    {
        [JsonProperty("end_offset")]
        public int EndOffset { get; internal set; }

        [JsonProperty("payload")]
        public string Payload { get; internal set; }

        [JsonProperty("position")]
        public int Position { get; internal set; }

        [JsonProperty("start_offset")]
        public int StartOffset { get; internal set; }
    }
}