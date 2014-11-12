﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Xml.Linq;
using Transformalize.Libs.SemanticLogging.Observable;
using Transformalize.Libs.SemanticLogging.Utility;

namespace Transformalize.Libs.SemanticLogging.Configuration
{
    internal class ElasticsearchSinkElement : ISinkElement
    {
        private readonly XName sinkName = XName.Get("elasticsearchSink", Constants.Namespace);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated with Guard class")]
        public bool CanCreateSink(XElement element)
        {
            Guard.ArgumentNotNull(element, "element");

            return element.Name == sinkName;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated with Guard class")]
        public IObserver<EventEntry> CreateSink(XElement element)
        {
            Guard.ArgumentNotNull(element, "element");

            var subject = new EventEntrySubject();
            subject.LogToElasticsearch(
                (string)element.Attribute("instanceName"),
                (string)element.Attribute("connectionString"),
                (string)element.Attribute("index") ?? "logstash",
                (string)element.Attribute("type") ?? "etw",
                (bool?)element.Attribute("flattenPayload") ?? true,
                element.Attribute("bufferingIntervalInSeconds").ToTimeSpan(),
                element.Attribute("bufferingFlushAllTimeoutInSeconds").ToTimeSpan() ??
                Constants.DefaultBufferingFlushAllTimeout,
                (int?)element.Attribute("bufferingCount") ?? Buffering.DefaultBufferingCount,
                (int?)element.Attribute("maxBufferSize") ?? Buffering.DefaultMaxBufferSize);

            return subject;
        }
    }
}