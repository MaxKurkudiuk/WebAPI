namespace WebAPI.Application.Jobs.Processing.Validation;

public sealed class NumericRangeValidator : IRowValidator {
    private readonly string _fieldName;
    private readonly decimal _min;

    public NumericRangeValidator(string fieldName, decimal min) {
        _fieldName = fieldName;
        _min = min;
    }

    public ValidationResult Validate(
        IReadOnlyDictionary<string, string> row) {
        if (!row.TryGetValue(_fieldName, out var value) ||
            !decimal.TryParse(value, out var number) ||
            number < _min) {
            return new ValidationResult {
                Errors = [
                    $"Field '{_fieldName}' must be >= {_min}."
                ]
            };
        }

        return new ValidationResult();
    }
}
