using FluentValidation;
using Storage.Infrastructure.DTOs;

namespace Storage.Api.Validations
{
    public class CreateStorageValidator : AbstractValidator<StorageApiCreate>
    {
        public CreateStorageValidator()
        {
            RuleFor(x => x.AddrStreet)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(120)
                .WithMessage("street addr can be contains 5-120 symbols");

            RuleFor(x => x.AddrCity)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(120)
                .WithMessage("city addr can be contains 5-120 symbols");

            RuleFor(x => x.AddrBuilding)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(60)
                .WithMessage("street addr can be contains 5-60 symbols");
        }
    }
}
