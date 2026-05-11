namespace WebAPI_01.Domain.Jobs; 

public sealed class Job {
    public Guid Id { get; init; } = Guid.NewGuid();
    public string JobType { get; init; } = default!;
    public IDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();
    public string Status { get; set; } = "Queued";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
