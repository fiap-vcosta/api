namespace Api.Extensions;

public static class WebApplicationExtensions
{
    public static void UseApiConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
