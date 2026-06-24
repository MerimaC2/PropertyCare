using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Application.Modules.Interventions.Commands.AddWorkLog;

public sealed class AddWorkLogCommandValidator : AbstractValidator<AddWorkLogCommand>
{
    public AddWorkLogCommandValidator()
    {
        RuleFor(x => x.WorkOrderId)
            .GreaterThan(0).WithMessage("Work order is required.");

        RuleFor(x => x.Note)
            .NotEmpty().WithMessage("Note is required.")
            .MaximumLength(WorkLogEntity.Constraints.NoteMaxLength);

        RuleFor(x => x.MinutesSpent)
            .GreaterThan(0).WithMessage("Time spent must be greater than 0.");
    }
}
