using DotnetApi.Endpoint.Api;

namespace DotnetApi.Endpoint;

public static class ApiMapper
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api");

        group.MapUserApiGroup();

        return app;
    }
}