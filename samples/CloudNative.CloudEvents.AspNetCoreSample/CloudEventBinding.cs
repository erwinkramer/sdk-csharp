using CloudNative.CloudEvents.AspNetCore;
using CloudNative.CloudEvents.Core;
using CloudNative.CloudEvents.NewtonsoftJson;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace CloudNative.CloudEvents.AspNetCoreSample;

public class CloudEventBinding : IBindableFromHttpContext<CloudEventBinding>
{
    private static readonly CloudEventFormatter formatter = new JsonEventFormatter();

    public CloudEvent Value { get; init; } = default!;

    public static async ValueTask<CloudEventBinding> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        Validation.CheckNotNull(context, nameof(context));
        Validation.CheckNotNull(parameter, nameof(parameter));

        var request = context.Request;

        var cloudEvent = await request.ToCloudEventAsync(formatter);
        
        return new CloudEventBinding
        {
            Value = cloudEvent
        };
    }
}