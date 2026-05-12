namespace WebAPI.Application.Jobs.Processing.Validation;

public sealed class RowValidationEngine {
    private readonly IReadOnlyCollection<IRowValidator> _validators;

    public RowValidationEngine(IEnumerable<IRowValidator> validators) {
        _validators = [.. validators];
    }

    public ValidationResult Validate(
        IReadOnlyDictionary<string, string> row) {
        var errors = _validators
            .Select(v => v.Validate(row))
            .SelectMany(r => r.Errors)
            .ToList();

        return new ValidationResult { Errors = errors };
    }
}
