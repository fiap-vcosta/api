namespace Api.Extensions;

public static class WebApplicationExtensions
{
    public static void UseApiConfiguration(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
