namespace PropertyCare.Application.Modules.Auth.Commands.Refresh;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");

        RuleFor(x => x.Fingerprint)
            .MaximumLength(256)
            .When(x => x.Fingerprint is not null);
    }
}
