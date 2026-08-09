using PropertyCare.Domain.Entities.Facilities;

namespace PropertyCare.Application.Modules.Facilities.Assets.Commands.Update;

public sealed class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(AssetEntity.Constraints.NameMaxLength);

        RuleFor(x => x.AssetTypeId)
            .GreaterThan(0).WithMessage("Asset type is required.");
    }
}
