using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Create;

public sealed class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("Unit is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(AssetEntity.Constraints.NameMaxLength);

        RuleFor(x => x.AssetTypeId)
            .GreaterThan(0).WithMessage("Asset type is required.");
    }
}
