using WebAPI_01.Domain.Jobs;

namespace WebAPI_01.Application.Interfaces; 

public interface IJobQueue {
    void Enqueue(Job job);
    ValueTask<Job> DequeueAsync(CancellationToken cancellationToken);
}
