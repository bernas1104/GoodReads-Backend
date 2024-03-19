using System.Net;
using System.Text.Json;

using FluentValidation.Results;

using GoodReads.Api.Middlewares;
using GoodReads.Domain.Common.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute.ExceptionExtensions;

namespace GoodReads.Unit.Tests.Api.Middlewares
{
    public class ExceptionsMiddlewareTest
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionsMiddleware _middleware;
        private readonly HttpContext _context;
        private readonly HttpResponse _response;

        public ExceptionsMiddlewareTest()
        {
            _next = Substitute.For<RequestDelegate>();

            _middleware = new (_next);

            _context = new DefaultHttpContext();
            _context.Request.Path = new PathString("/some-path");

            _response = Substitute.For<HttpResponse>();
        }

        [Theory]
        [InlineData(typeof(DomainException))]
        [InlineData(typeof(Exception))]
        public async Task GivenInvoke_WhenInvokeThrowsGenericException_ShouldReturnProblemDetailsResponse(
            Type type
        )
        {
            // arrange
            var exception = type == typeof(DomainException) ?
                new DomainException("Some domain exception") :
                new Exception("Some internal exception");

            var expectedStatusCode = type == typeof(DomainException) ?
                (int)HttpStatusCode.BadRequest :
                (int)HttpStatusCode.InternalServerError;

            _context.Response.Body = new MemoryStream();

            _next.Invoke(Arg.Any<HttpContext>()).ThrowsAsync(exception);

            // act
            await _middleware.Invoke(_context);

            _context.Response.Body.Position = 0;

            var reader = new StreamReader(_context.Response.Body);
            var data = await reader.ReadToEndAsync();
            var response = JsonSerializer.Deserialize<IEnumerable<ProblemDetails>>(data);

            // assert
            response.Should().NotBeNull();
            response.Should().BeOfType<List<ProblemDetails>>();

            _context.Response.ContentType.Should()
                .Be("application/problem+json; charset=utf-8");
            _context.Response.StatusCode.Should().Be(expectedStatusCode);
        }

        [Fact]
        public async Task GivenInvoke_WhenInvokeThrowsValidationException_ShouldReturnProblemDetailsResponse()
        {
            // arrange
            var exception = new FluentValidation.ValidationException(
                "Some validation error",
                new List<ValidationFailure>
                {
                    new ValidationFailure("SomeProperty", "SomeErrorMessage")
                }
            );

            var expectedStatusCode = (int)HttpStatusCode.BadRequest;

            _context.Response.Body = new MemoryStream();

            _next.Invoke(Arg.Any<HttpContext>()).ThrowsAsync(exception);

            // act
            await _middleware.Invoke(_context);

            _context.Response.Body.Position = 0;

            var reader = new StreamReader(_context.Response.Body);
            var data = await reader.ReadToEndAsync();
            var response = JsonSerializer.Deserialize<IEnumerable<ProblemDetails>>(data);

            // assert
            response.Should().NotBeNull();
            response.Should().BeOfType<List<ProblemDetails>>();

            _context.Response.ContentType.Should()
                .Be("application/problem+json; charset=utf-8");
            _context.Response.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}