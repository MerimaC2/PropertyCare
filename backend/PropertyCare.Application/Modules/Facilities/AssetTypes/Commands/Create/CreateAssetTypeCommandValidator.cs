using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Application.Modules.Facilities.AssetTypes.Commands.Create;

public sealed class CreateAssetTypeCommandValidator : AbstractValidator<CreateAssetTypeCommand>
{
    public CreateAssetTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(AssetTypeEntity.Constraints.NameMaxLength);

        RuleFor(x => x.DefaultSlaHours)
            .GreaterThan(0)
            .When(x => x.DefaultSlaHours.HasValue)
            .WithMessage("SLA hours must be greater than 0.");
    }
}
