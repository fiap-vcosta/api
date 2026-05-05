using System.Collections.Generic;
using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Api.Middlewares;

public sealed class UserRoleValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<TipoUsuario> _allowedRoles;

    public UserRoleValidationMiddleware(RequestDelegate next, params TipoUsuario[]? allowedRoles)
    {
        _next = next;
        _allowedRoles = allowedRoles is null
            ? new HashSet<TipoUsuario>()
            : new HashSet<TipoUsuario>(allowedRoles);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authenticateResult = await context.AuthenticateAsync();
        if (!authenticateResult.Succeeded || authenticateResult.Principal?.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        var roleClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleClaim)
            || !Enum.TryParse<TipoUsuario>(roleClaim, ignoreCase: true, out var userTipo)
            || !_allowedRoles.Contains(userTipo))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        await _next(context);
    }
}
