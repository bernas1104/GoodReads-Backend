using FluentValidation;

using GoodReads.Application.Common.Behaviours;

using MediatR;

namespace GoodReads.Unit.Tests.Application.Common.Behaviours
{
    public class ValidationBehaviourTest
    {
        private readonly List<IValidator<FooRequest>> _validators;
        private readonly ValidationBehaviour<FooRequest, MediatR.Unit> _behaviour;
        private readonly RequestHandlerDelegate<MediatR.Unit> _next;

        public ValidationBehaviourTest()
        {
            _validators = new List<IValidator<FooRequest>>();
            _behaviour = new (_validators);
            _next = Substitute.For<RequestHandlerDelegate<MediatR.Unit>>();
        }

        [Fact]
        public async Task GivenValidationBehaviourHandler_WhenNoValidator_ShouldReturnResponse()
        {
            // arrange
            var request = new FooRequest("Bar");

            _next().Returns(MediatR.Unit.Value);

            // act
            var result = await _behaviour.Handle(
                request,
                _next,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenValidationBehaviourHandler_WhenValidatorAvailableAndValidRequest_ShouldReturnResponse()
        {
            // arrange
            var request = new FooRequest("Bar");

            _validators.Add(new FooValidator());

            _next().Returns(MediatR.Unit.Value);

            // act
            var result = await _behaviour.Handle(
                request,
                _next,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenValidationBehaviourHandler_WhenValidatorAvailableAndInvalidRequest_ShouldThrowValidationException()
        {
            // arrange
            var request = new FooRequest("");

            _validators.Add(new FooValidator());

            _next().Returns(MediatR.Unit.Value);

            // act
            var result = async () => await _behaviour.Handle(
                request,
                _next,
                CancellationToken.None
            );

            // assert
            await result.Should()
                .ThrowAsync<FluentValidation.ValidationException>();
        }
    }

    internal sealed record FooRequest(string Bar) : IRequest<MediatR.Unit>;

    internal sealed class FooValidator : AbstractValidator<FooRequest>
    {
        public FooValidator()
        {
            RuleFor(x => x.Bar).NotEmpty();
        }
    }
}