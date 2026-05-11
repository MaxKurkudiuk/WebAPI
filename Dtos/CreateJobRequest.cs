namespace WebAPI_01.Dtos; 

public sealed class CreateJobRequest {
    public string JobType { get; init; } = default!;
    public Dictionary<string, string> Parameters { get; init; } = new();
}
