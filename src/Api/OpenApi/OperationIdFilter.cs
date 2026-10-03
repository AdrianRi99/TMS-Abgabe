using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi;

namespace TMS.Api.OpenApi;

public sealed class OperationIdFilter : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken ct)
    {
        var endpointName = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<EndpointNameMetadata>()
            .FirstOrDefault();

        if (endpointName is not null)
            operation.OperationId = endpointName.EndpointName;

        return Task.CompletedTask;
    }
}