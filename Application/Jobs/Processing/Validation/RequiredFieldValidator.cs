namespace WebAPI.Application.Jobs.Processing.Validation;

public sealed class RequiredFieldValidator : IRowValidator {
    private readonly string _fieldName;

    public RequiredFieldValidator(string fieldName) {
        _fieldName = fieldName;
    }

    public ValidationResult Validate(
        IReadOnlyDictionary<string, string> row) {
        if (!row.TryGetValue(_fieldName, out var value)
            || string.IsNullOrWhiteSpace(value)) {
            return new ValidationResult {
                Errors = [$"Field '{_fieldName}' is required."]
            };
        }

        return new ValidationResult();
    }
}
