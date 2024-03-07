using System.Diagnostics.CodeAnalysis;
using System.Net;

using ErrorOr;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GoodReads.Api.Filters
{
    public sealed class ErrorOrResponseFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;
            var response = result?.Value;

            if (response is not null && IsErrorOrResponse(response))
            {
                object newResultValue;
                int? statusCode = result?.StatusCode;

                if (IsError(response))
                {
                    var error = GetError(response);
                    statusCode = GetStatusCode(error);
                    newResultValue = new ProblemDetails
                    {
                        Title = error.Code,
                        Detail = error.Description,
                        Status = statusCode,
                        Instance = context.HttpContext.Request.Path
                    };
                }
                else
                {
                    newResultValue = GetValue(response);
                }

                context.Result = new ObjectResult(newResultValue)
                {
                    StatusCode = statusCode,
                };
            }
        }

        [ExcludeFromCodeCoverage]
        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Noop
        }

        private bool IsErrorOrResponse(object response) => response.GetType()
            .Namespace!.Equals("ErrorOr");

        private bool IsError(object response) => (bool)response.GetType()
            .GetProperty("IsError")!
            .GetValue(response)!;

        private Error GetError(object response) => (Error)response.GetType()
            .GetProperty("FirstError")!
            .GetValue(response)!;

        private int GetStatusCode(Error error)
        {
            var errorType = error.Type;

            switch (errorType)
            {
                case ErrorType.NotFound:
                    return (int)HttpStatusCode.NotFound;
                case ErrorType.Conflict:
                    return (int)HttpStatusCode.Conflict;
                default:
                    return (int)HttpStatusCode.InternalServerError;
            }
        }

        private object GetValue(object response) => response.GetType()
            .GetProperty("Value")!
            .GetValue(response)!;
    }
}