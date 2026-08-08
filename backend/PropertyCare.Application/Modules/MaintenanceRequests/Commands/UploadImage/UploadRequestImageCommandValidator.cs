using PropertyCare.Domain.Entities.Maintenance;

namespace PropertyCare.Application.Modules.MaintenanceRequests.Commands.UploadImage;

public sealed class UploadRequestImageCommandValidator
    : AbstractValidator<UploadRequestImageCommand>
{
    public UploadRequestImageCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("Request is required.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(RequestImageEntity.Constraints.FileNameMaxLength);

        RuleFor(x => x.ContentType)
            .Must(ct => RequestImageEntity.Constraints.AllowedContentTypes.Contains(ct))
            .WithMessage("Only JPEG, PNG or WebP images are allowed.");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0).WithMessage("File is empty.")
            .LessThanOrEqualTo(RequestImageEntity.Constraints.MaxSizeBytes)
            .WithMessage("Image must be 5 MB or smaller.");
    }
}
