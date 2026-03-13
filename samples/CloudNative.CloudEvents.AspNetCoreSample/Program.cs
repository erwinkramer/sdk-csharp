// Copyright (c) Cloud Native Foundation.
// Licensed under the Apache 2.0 license.
// See LICENSE file in the project root for full license information.

using CloudNative.CloudEvents.AspNetCoreSample;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var apiEvents = app.MapGroup("/api/events");
apiEvents.MapPost("/receive", CloudEventController.ReceiveCloudEvent);
apiEvents.MapGet("/generate", CloudEventController.GenerateCloudEvent);

app.Run();

// Generated `Program` class when using top-level statements
// is internal by default. Make this `public` here for tests.
public partial class Program { }
