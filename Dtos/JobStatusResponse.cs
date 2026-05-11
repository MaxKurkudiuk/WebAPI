using WebAPI.Domain.Jobs;

namespace WebAPI.Dtos;

public sealed class JobStatusResponse {
    public Guid JobId { get; init; }
    public JobState State { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }
}
