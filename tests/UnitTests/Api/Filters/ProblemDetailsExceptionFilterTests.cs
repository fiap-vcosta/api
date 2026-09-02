using Api.Filters;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Api.Filters;

public class ProblemDetailsExceptionFilterTests
{
    private readonly ProblemDetailsExceptionFilter _filter = new();

    [Fact]
    public void OnException_Returns404ProblemDetails_WhenDomainNotFoundException()
    {
        // Arrange
        var context = CreateExceptionContext(new DomainNotFoundException("Cliente não encontrado"));

        // Act
        _filter.OnException(context);

        // Assert
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
        Assert.Equal("Not Found", problem.Title);
        Assert.Equal("Cliente não encontrado", problem.Detail);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Returns404ProblemDetails_WhenKeyNotFoundException()
    {
        // Arrange
        var context = CreateExceptionContext(new KeyNotFoundException("Recurso não encontrado"));

        // Act
        _filter.OnException(context);

        // Assert
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal("Recurso não encontrado", problem.Detail);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Returns400ProblemDetails_WhenBusinessRuleException()
    {
        // Arrange
        var context = CreateExceptionContext(new BusinessRuleException("Documento duplicado"));

        // Act
        _filter.OnException(context);

        // Assert
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("Bad Request", problem.Title);
        Assert.Equal("Documento duplicado", problem.Detail);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Returns400ProblemDetails_WhenInvalidOperationException()
    {
        // Arrange
        var context = CreateExceptionContext(new InvalidOperationException("Operação inválida"));

        // Act
        _filter.OnException(context);

        // Assert
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal("Operação inválida", problem.Detail);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Returns401ProblemDetails_WhenUnauthorizedAccessException()
    {
        // Arrange
        var context = CreateExceptionContext(new UnauthorizedAccessException("Login ou senha inválidos."));

        // Act
        _filter.OnException(context);

        // Assert
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status401Unauthorized, problem.Status);
        Assert.Equal("Unauthorized", problem.Title);
        Assert.Equal("Login ou senha inválidos.", problem.Detail);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Returns500ProblemDetails_WhenUnhandledException()
    {
        // Arrange
        var context = CreateExceptionContext(new NotSupportedException("detalhe interno"));

        // Act
        _filter.OnException(context);

        // Assert
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
        Assert.Equal("An error occurred while processing your request.", problem.Title);
        Assert.Equal("An unexpected error occurred.", problem.Detail);
        Assert.True(context.ExceptionHandled);
    }

    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, [])
        {
            Exception = exception
        };
    }
}
