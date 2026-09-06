namespace PropertyCare.Application.Modules.Facilities.Buildings.Queries.List;

public sealed class ListBuildingsQueryValidator : AbstractValidator<ListBuildingsQuery>
{
    public ListBuildingsQueryValidator()
    {
        RuleFor(x => x.MinUnitCount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinUnitCount.HasValue)
            .WithMessage("Minimum unit count cannot be negative.");

        RuleFor(x => x.MaxUnitCount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxUnitCount.HasValue)
            .WithMessage("Maximum unit count cannot be negative.");

        RuleFor(x => x.MaxUnitCount)
            .GreaterThanOrEqualTo(x => x.MinUnitCount!.Value)
            .When(x => x.MinUnitCount.HasValue && x.MaxUnitCount.HasValue)
            .WithMessage("Maximum unit count cannot be lower than the minimum.");
    }
}
