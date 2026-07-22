using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class ProblemDetailsExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled)
        {
            return;
        }

        var (statusCode, title, detail) = MapException(context.Exception);

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        })
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            DomainNotFoundException or KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                exception.Message),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                exception.Message),
            BusinessRuleException or InvalidOperationException or ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An error occurred while processing your request.",
                "An unexpected error occurred.")
        };
    }
}
