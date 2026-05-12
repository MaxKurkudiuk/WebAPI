namespace WebAPI.Application.Jobs.Processing.Validation;

public sealed class ValidationResult {
    public bool IsValid => !Errors.Any();
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}
