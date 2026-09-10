using System.Collections.Generic;
using Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Api.Filters;

public class ServiceKeyAuthorizeAttributeTests
{
    [Fact]
    public void OnAuthorization_SetsUnauthorized_WhenKeyMissing()
    {
        // Arrange
        var context = CreateContext(null, "expected-key");
        var filter = new ServiceKeyAuthorizeAttribute();

        // Act
        filter.OnAuthorization(context);

        // Assert
        Assert.IsType<UnauthorizedResult>(context.Result);
    }

    [Fact]
    public void OnAuthorization_SetsUnauthorized_WhenKeyWrong()
    {
        // Arrange
        var context = CreateContext("wrong", "expected-key");
        var filter = new ServiceKeyAuthorizeAttribute();

        // Act
        filter.OnAuthorization(context);

        // Assert
        Assert.IsType<UnauthorizedResult>(context.Result);
    }

    [Fact]
    public void OnAuthorization_Allows_WhenKeyMatches()
    {
        // Arrange
        var context = CreateContext("expected-key", "expected-key");
        var filter = new ServiceKeyAuthorizeAttribute();

        // Act
        filter.OnAuthorization(context);

        // Assert
        Assert.Null(context.Result);
    }

    private static AuthorizationFilterContext CreateContext(string? providedKey, string expectedKey)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ServiceAuth:Key"] = expectedKey
            })
            .Build());
        var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        if (providedKey is not null)
        {
            httpContext.Request.Headers[ServiceKeyAuthorizeAttribute.HeaderName] = providedKey;
        }

        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, []);
    }
}
