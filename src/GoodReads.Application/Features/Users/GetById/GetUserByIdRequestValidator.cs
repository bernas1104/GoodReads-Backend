using FluentValidation;

namespace GoodReads.Application.Features.Users.GetById
{
    public sealed class GetUserByIdRequestValidator : AbstractValidator<GetUserByIdRequest>
    {
        public GetUserByIdRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}