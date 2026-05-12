namespace WebAPI.Application.Jobs.Processing.Validation;

public sealed class DateValidator : IRowValidator {
    private readonly string _fieldName;

    public DateValidator(string fieldName) {
        _fieldName = fieldName;
    }

    public ValidationResult Validate(
        IReadOnlyDictionary<string, string> row) {
        if (!row.TryGetValue(_fieldName, out var value) ||
            !DateTime.TryParse(value, out _)) {
            return new ValidationResult {
                Errors = [
                    $"Field '{_fieldName}' must be a valid date."
                ]
            };
        }

        return new ValidationResult();
    }
}
