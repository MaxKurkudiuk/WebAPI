namespace WebAPI_01.Dtos; 

public sealed class CreateJobResponse {
    public Guid JobId { get; init; }
    public string Status { get; init; } = "Queued";
}
