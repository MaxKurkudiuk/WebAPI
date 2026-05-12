using DocumentFormat.OpenXml.Spreadsheet;
using WebAPI.Application.Jobs.Processing.Validation;

namespace WebAPI.Application.Jobs.Processing;

public sealed class CsvFileProcessor : IFileProcessor {
    private readonly RowValidationEngine _validator;

    public CsvFileProcessor(RowValidationEngine validator) {
        _validator = validator;
    }

    public bool CanProcess(string filePath)
        => Path.GetExtension(filePath)
            .Equals(".csv", StringComparison.OrdinalIgnoreCase);

    public async Task<FileProcessingResult> ProcessAsync(
            string filePath,
            CancellationToken cancellationToken) 
        {
        var result = new FileProcessingResult();
        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
        var headers = lines[0].Split(',');

        var processed = 0;
        var valid = 0;

        foreach (var line in lines.Skip(1)) // skip header
        {
            processed++;

            var values = line.Split(',');
            var row = headers.Zip(values,
                (h, v) => new { h, v })
                .ToDictionary(x => x.h, x => x.v);

            var validation = _validator.Validate(row);

            if (validation.IsValid) {
                valid++;
            }
        }

        return new FileProcessingResult {
            RowsProcessed = processed,
            RowsValid = valid,
            RowsInvalid = processed - valid
        };
    }
}