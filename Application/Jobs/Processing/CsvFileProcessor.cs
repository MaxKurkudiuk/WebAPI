namespace WebAPI.Application.Jobs.Processing;

public sealed class CsvFileProcessor : IFileProcessor {
    public bool CanProcess(string filePath)
        => Path.GetExtension(filePath)
            .Equals(".csv", StringComparison.OrdinalIgnoreCase);

    public async Task<FileProcessingResult> ProcessAsync(
        string filePath,
        CancellationToken cancellationToken) {
        var result = new FileProcessingResult();
        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);

        var processed = 0;
        var valid = 0;

        foreach (var line in lines.Skip(1)) // skip header
        {
            processed++;

            if (!string.IsNullOrWhiteSpace(line)) {
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