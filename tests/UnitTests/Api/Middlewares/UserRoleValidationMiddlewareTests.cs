using Api.Middlewares;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Claims;

namespace UnitTests.Api.Middlewares;

public class UserRoleValidationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_AllowsRequest_WhenUserRoleIsAllowed()
    {
        // Arrange
        var authService = new TestAuthenticationService(
            AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, TipoUsuario.Admin.ToString())
                }, "Test")), "Test")));

        var context = CreateHttpContext(authService);
        var nextCalled = false;
        RequestDelegate next = ctx =>
        {
            nextCalled = true;
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        };

        var middleware = new UserRoleValidationMiddleware(next, TipoUsuario.Admin);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsForbidden_WhenUserRoleIsNotAllowed()
    {
        // Arrange
        var authService = new TestAuthenticationService(
            AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, TipoUsuario.Atendente.ToString())
                }, "Test")), "Test")));

        var context = CreateHttpContext(authService);
        RequestDelegate next = _ => throw new InvalidOperationException("Next should not be called");
        var middleware = new UserRoleValidationMiddleware(next, TipoUsuario.Admin);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.Equal("Forbidden", await ReadResponseBodyAsync(context));
    }

    [Fact]
    public async Task InvokeAsync_ReturnsUnauthorized_WhenAuthenticationFails()
    {
        // Arrange
        var authService = new TestAuthenticationService(AuthenticateResult.Fail("Invalid token"));
        var context = CreateHttpContext(authService);
        RequestDelegate next = _ => throw new InvalidOperationException("Next should not be called");
        var middleware = new UserRoleValidationMiddleware(next, TipoUsuario.Admin);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.Equal("Unauthorized", await ReadResponseBodyAsync(context));
    }

    private static DefaultHttpContext CreateHttpContext(IAuthenticationService authService)
    {
        var services = new ServiceCollection();
        services.AddSingleton(authService);

        var context = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadResponseBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    private sealed class TestAuthenticationService : IAuthenticationService
    {
        private readonly AuthenticateResult _result;

        public TestAuthenticationService(AuthenticateResult result)
        {
            _result = result;
        }

        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
            => Task.FromResult(_result);

        public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            => Task.CompletedTask;

        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            => Task.CompletedTask;

        public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
            => Task.CompletedTask;

        public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
            => Task.CompletedTask;
    }
}
