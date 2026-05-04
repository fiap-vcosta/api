namespace Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApiConfiguration(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}
