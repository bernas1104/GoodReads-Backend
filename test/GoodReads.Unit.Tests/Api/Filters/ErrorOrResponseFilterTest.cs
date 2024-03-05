using ErrorOr;

using GoodReads.Api.Filters;
using GoodReads.Application.Features.Users.GetById;
using GoodReads.Shared.Mocks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace GoodReads.Unit.Tests.Api.Filters
{
    public class ErrorOrResponseFilterTest
    {
        private readonly ErrorOrResponseFilter _filter;
        private ResultExecutingContext? _context;
        private ActionContext? _actionContext;
        // private readonly IActionResult _actionResult;

        public ErrorOrResponseFilterTest()
        {
            _filter = new ();
        }

        [Theory]
        [InlineData("Custom.NotFound")]
        [InlineData("Custom.Conflict")]
        [InlineData("Custom.Custom")]
        public void GivenInvoke_WhenResponseIsTypeErrorOrAndIsError_ShouldReplaceResponseWithProblemDetails(
            string code
        )
        {
            // arrange
            ErrorOr<object> response = Error.Failure(code: code);

            var httpContext = Substitute.For<HttpContext>();
            var request = Substitute.For<HttpRequest>();

            httpContext.Request.Returns(request);
            request.Path.Returns(new PathString("/somePath"));

            _actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = Substitute.For<RouteData>(),
                ActionDescriptor = Substitute.For<ActionDescriptor>()
            };

            _context = new ResultExecutingContext(
                actionContext: _actionContext,
                filters: Substitute.For<IList<IFilterMetadata>>(),
                result: new OkObjectResult(response),
                controller: Substitute.For<object>()
            );

            // act
            _filter.OnResultExecuting(_context);

            // assert
            ((ObjectResult)_context.Result).Value.Should().BeOfType<ProblemDetails>();
        }

        [Fact]
        public void GivenInvoke_WhenResponseIsTypeErrorOrAndIsNotError_ShouldReplaceResponseWithValue()
        {
            // arrange
            var getUserByIdResponse = UserMock.GetUserByIdResponse();
            ErrorOr<GetUserByIdResponse> response = getUserByIdResponse;

            var httpContext = Substitute.For<HttpContext>();
            var request = Substitute.For<HttpRequest>();

            httpContext.Request.Returns(request);
            request.Path.Returns(new PathString("/somePath"));

            _actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = Substitute.For<RouteData>(),
                ActionDescriptor = Substitute.For<ActionDescriptor>()
            };

            _context = new ResultExecutingContext(
                actionContext: _actionContext,
                filters: Substitute.For<IList<IFilterMetadata>>(),
                result: new OkObjectResult(response),
                controller: Substitute.For<object>()
            );

            // act
            _filter.OnResultExecuting(_context);

            // assert
            ((ObjectResult)_context.Result).Value.Should().BeOfType<GetUserByIdResponse>();
        }

        [Fact]
        public void GivenInvoke_WhenResponseIsNotTypeError_ShouldNotReplaceResponse()
        {
            // arrange
            var response = UserMock.GetUserByIdResponse();

            var httpContext = Substitute.For<HttpContext>();
            var request = Substitute.For<HttpRequest>();

            httpContext.Request.Returns(request);
            request.Path.Returns(new PathString("/somePath"));

            _actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = Substitute.For<RouteData>(),
                ActionDescriptor = Substitute.For<ActionDescriptor>()
            };

            _context = new ResultExecutingContext(
                actionContext: _actionContext,
                filters: Substitute.For<IList<IFilterMetadata>>(),
                result: new OkObjectResult(response),
                controller: Substitute.For<object>()
            );

            // act
            _filter.OnResultExecuting(_context);

            // assert
            ((ObjectResult)_context.Result).Value.Should().BeOfType<GetUserByIdResponse>();
        }
    }
}