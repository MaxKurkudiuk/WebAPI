namespace WebAPI.Application.Jobs.Processing.Validation;

public interface IRowValidator {
    ValidationResult Validate(
        IReadOnlyDictionary<string, string> row);
}
