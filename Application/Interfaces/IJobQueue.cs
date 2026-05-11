using WebAPI.Domain.Jobs;

namespace WebAPI.Application.Interfaces; 

public interface IJobQueue {
    void Enqueue(Job job);
    ValueTask<Job> DequeueAsync(CancellationToken cancellationToken);
}
