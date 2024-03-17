using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.Json;

using FluentValidation;

using GoodReads.Domain.Common.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Middlewares
{
    public sealed class ExceptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        //[ExcludeFromCodeCoverage]
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json+problem; charset=utf-8";

            context.Response.StatusCode = (int)MapHttpStatusCode(ex);

            var response = GetErrorResponse(context, ex);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static HttpStatusCode MapHttpStatusCode(Exception ex)
        {
            return ex switch
            {
                var e when e is DomainException => HttpStatusCode.BadRequest,
                var e when e is ValidationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static IEnumerable<ProblemDetails> GetErrorResponse(
            HttpContext context,
            Exception ex
        )
        {
            return ex switch
            {
                var e when e is ValidationException =>
                    GetValidationErrorResponse(context, (e as ValidationException)!),
                _ => GetGenericErrorResponse(context, ex, ex.GetType().ToString())
            };
        }

        private static IEnumerable<ProblemDetails> GetValidationErrorResponse(
            HttpContext context,
            ValidationException ex
        )
        {
            return ex.Errors.Select(
                e => new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = new StringBuilder()
                        .Append("An error occurred when validating the data. ")
                        .Append(e.PropertyName + ". ")
                        .Append(e.ErrorMessage)
                        .ToString(),
                    Instance = context.Request.Path,
                    Status = (int)HttpStatusCode.BadRequest,
                }
            );
        }

        private static IEnumerable<ProblemDetails> GetGenericErrorResponse(
            HttpContext context,
            Exception ex,
            string type
        )
        {
            yield return new ProblemDetails
            {
                Title = "Error",
                Detail = new StringBuilder()
                    .Append("An error occurred. ")
                    .Append(ex.Message + ". ")
                    .ToString(),
                Instance = context.Request.Path,
                Status = ex.GetType() == typeof(DomainException) ?
                    (int)HttpStatusCode.BadRequest :
                    (int)HttpStatusCode.InternalServerError,
                Type = type
            };
        }
    }
}