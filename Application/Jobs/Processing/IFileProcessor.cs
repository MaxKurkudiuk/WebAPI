namespace WebAPI.Application.Jobs.Processing;

public interface IFileProcessor {
    bool CanProcess(string filePath);
    Task<FileProcessingResult> ProcessAsync(
        string filePath,
        CancellationToken cancellationToken);
}
