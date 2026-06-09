using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Commands.Create;

public sealed class CreateMaintenanceRequestCommandValidator
    : AbstractValidator<CreateMaintenanceRequestCommand>
{
    public CreateMaintenanceRequestCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long.")
            .MaximumLength(MaintenanceRequestEntity.Constraints.TitleMaxLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(MaintenanceRequestEntity.Constraints.DescriptionMaxLength);

        RuleFor(x => x.BuildingId)
            .GreaterThan(0).WithMessage("Building is required.");

        RuleFor(x => x.PriorityId)
            .GreaterThan(0).WithMessage("Priority is required.");

        RuleFor(x => x.UnitId)
            .GreaterThan(0)
            .When(x => x.UnitId.HasValue)
            .WithMessage("Unit is not valid.");

        RuleFor(x => x.AssetId)
            .GreaterThan(0)
            .When(x => x.AssetId.HasValue)
            .WithMessage("Asset is not valid.");

        // An asset can only be selected together with its unit.
        RuleFor(x => x.UnitId)
            .NotNull()
            .When(x => x.AssetId.HasValue)
            .WithMessage("Unit is required when an asset is selected.");
    }
}
