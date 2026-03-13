using CloudNative.CloudEvents.AspNetCore;
using CloudNative.CloudEvents.Core;
using CloudNative.CloudEvents.SystemTextJson;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;

namespace CloudNative.CloudEvents.AspNetCoreSample.Bindings;

public class CloudEventBinding : IBindableFromHttpContext<CloudEventBinding>
{
    public CloudEvent Value { get; init; } = default!;

    public static async ValueTask<CloudEventBinding> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        Validation.CheckNotNull(context, nameof(context));
        Validation.CheckNotNull(parameter, nameof(parameter));

        var request = context.Request;
        var formatter = context.RequestServices.GetRequiredService<JsonEventFormatter>();

        var cloudEvent = await request.ToCloudEventAsync(formatter);
        
        return new CloudEventBinding
        {
            Value = cloudEvent
        };
    }
}