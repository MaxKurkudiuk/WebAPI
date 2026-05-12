namespace WebAPI.Application.Jobs.Processing;

public sealed class FileProcessingResult {
    public int RowsProcessed { get; init; }
    public int RowsValid { get; init; }
    public int RowsInvalid { get; init; }
}
