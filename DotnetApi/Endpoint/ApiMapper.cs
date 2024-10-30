namespace DotnetApi.Endpoint;

public static class ApiMapper
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapGroup("/api");

        return app;
    }
}