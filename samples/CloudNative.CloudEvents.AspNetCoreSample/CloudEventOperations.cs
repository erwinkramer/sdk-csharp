// Copyright (c) Cloud Native Foundation.
// Licensed under the Apache 2.0 license.
// See LICENSE file in the project root for full license information.

using CloudNative.CloudEvents.AspNetCore;
using CloudNative.CloudEvents.SystemTextJson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudNative.CloudEvents.AspNetCoreSample
{
    public class CloudEventController
    {
        private static readonly CloudEventFormatter formatter = new JsonEventFormatter();

        public static async Task<JsonHttpResult<object>> ReceiveCloudEvent(CloudEventBinding cloudEvent)
        {
            var data = (JsonElement) cloudEvent.Value.Data;
            
            var attributes = new Dictionary<string, string>();
            foreach (var (attribute, value) in cloudEvent.Value.GetPopulatedAttributes())
            {
                attributes[attribute.Name] = attribute.Format(value);
            }

            return TypedResults.Json<object>(new
            {
                message = data.GetProperty("message").GetString(),
                version = cloudEvent.Value.SpecVersion.VersionId,
                id = cloudEvent.Value.Id,
                attributes
            });
        }

        /// <summary>
        /// Generates a CloudEvent in "structured mode", where all CloudEvent information is
        /// included within the body of the response.
        /// </summary>
        public static async Task GenerateCloudEvent(HttpResponse response)
        {
            var evt = new CloudEvent
            {
                Type = "CloudNative.CloudEvents.AspNetCoreSample",
                Source = new Uri("https://github.com/cloudevents/sdk-csharp"),
                Time = DateTimeOffset.Now,
                DataContentType = "application/json",
                Id = Guid.NewGuid().ToString(),
                Data = new
                {
                    Language = "C#",
                    EnvironmentVersion = Environment.Version.ToString()
                }
            };

            response.StatusCode = StatusCodes.Status200OK;
            await evt.CopyToHttpResponseAsync(response, ContentMode.Structured, formatter);
        }
    }
}
