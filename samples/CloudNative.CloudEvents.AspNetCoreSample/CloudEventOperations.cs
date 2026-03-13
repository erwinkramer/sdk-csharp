// Copyright (c) Cloud Native Foundation.
// Licensed under the Apache 2.0 license.
// See LICENSE file in the project root for full license information.

using CloudNative.CloudEvents.SystemTextJson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;
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
        public static async Task<ContentHttpResult> GenerateCloudEvent()
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

            // Format the event as the body of the response. This is UTF-8 JSON because of
            // the CloudEventFormatter we're using, but EncodeStructuredModeMessage always
            // returns binary data. We could return the data directly, but for debugging
            // purposes it's useful to have the JSON string.
            var bytes = formatter.EncodeStructuredModeMessage(evt, out var contentType);
            string json = Encoding.UTF8.GetString(bytes.Span);

            // Specify the content type of the response: this is what makes it a CloudEvent.
            // (In "binary mode", the content type is the content type of the data, and headers
            // indicate that it's a CloudEvent.)
            return TypedResults.Text(json, contentType.MediaType, statusCode: StatusCodes.Status200OK);
        }
    }
}
