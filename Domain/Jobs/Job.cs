namespace WebAPI.Domain.Jobs; 

public sealed class Job {
    public Guid Id { get; init; } = Guid.NewGuid();
    public string JobType { get; init; } = default!;
    public IDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();
    
    public JobState State { get; set; } = JobState.Pending;
    
    public string? CorrelationId { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
}
