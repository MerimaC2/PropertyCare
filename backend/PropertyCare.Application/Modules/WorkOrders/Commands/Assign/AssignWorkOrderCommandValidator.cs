using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Application.Modules.WorkOrders.Commands.Assign;

public sealed class AssignWorkOrderCommandValidator : AbstractValidator<AssignWorkOrderCommand>
{
    public AssignWorkOrderCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("Request is required.");

        RuleFor(x => x.AssignedToUserId)
            .GreaterThan(0).WithMessage("Technician is required.");

        RuleFor(x => x.Note)
            .MaximumLength(WorkOrderEntity.Constraints.NoteMaxLength)
            .When(x => x.Note is not null);
    }
}
