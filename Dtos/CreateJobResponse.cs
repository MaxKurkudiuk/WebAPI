using WebAPI.Domain.Jobs;

namespace WebAPI.Dtos; 

public sealed class CreateJobResponse {
    public Guid JobId { get; init; }
    public JobState State { get; init; }
}
