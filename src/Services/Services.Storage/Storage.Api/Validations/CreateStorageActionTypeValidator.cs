using FluentValidation;
using Storage.Infrastructure.DTOs;

namespace Storage.Api.Validations
{
    public class CreateStorageActionTypeValidator : AbstractValidator<StorageActionTypeApiCreate>
    {
        public CreateStorageActionTypeValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(60)
                .WithMessage("street addr can be contains 3-60 symbols");
        }
    }
}
