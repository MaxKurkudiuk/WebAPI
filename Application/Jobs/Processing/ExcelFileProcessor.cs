using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WebAPI.Application.Jobs.Processing;

public sealed class ExcelFileProcessor : IFileProcessor {
    public bool CanProcess(string filePath)
        => Path.GetExtension(filePath)
            .Equals(".xlsx", StringComparison.OrdinalIgnoreCase);

    public Task<FileProcessingResult> ProcessAsync(
        string filePath,
        CancellationToken cancellationToken) {
        int processed = 0;
        int valid = 0;

        using var document = SpreadsheetDocument.Open(filePath, false);
        var workbookPart = document.WorkbookPart!;
        var sheet = workbookPart.Workbook?.Sheets!
            .OfType<Sheet>()
            .First();

        var worksheetPart = (WorksheetPart)
            workbookPart.GetPartById(sheet?.Id!);

        var rows = worksheetPart.Worksheet?
            .Descendants<Row>()
            .Skip(1) ?? []; // skip header

        foreach (var row in rows) {
            processed++;

            if (row.Elements<Cell>().Any()) {
                valid++;
            }
        }

        return Task.FromResult(new FileProcessingResult {
            RowsProcessed = processed,
            RowsValid = valid,
            RowsInvalid = processed - valid
        });
    }
}
