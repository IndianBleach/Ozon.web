using FluentValidation;
using Storage.Infrastructure.DTOs;

namespace Storage.Api.Validations
{
    public class CreateStorageCellValidator : AbstractValidator<StorageCellApiCreate>
    {
        public CreateStorageCellValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(20)
                .WithMessage("cell name must be 5-20 symbols");

            RuleFor(x => x.Comment)
                .MaximumLength(120)
                .WithMessage("comment must be 0-120 symbols");
        }
    }
}
