using System.Threading.Channels;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Jobs;

namespace WebAPI.Infrastructure.Queues; 

public sealed class InMemoryJobQueue : IJobQueue {
    private readonly Channel<Job> _queue = Channel.CreateUnbounded<Job>();

    public void Enqueue(Job job) {
        if (!_queue.Writer.TryWrite(job))
            throw new InvalidOperationException("Failed to enqueue job.");
    }

    public async ValueTask<Job> DequeueAsync(CancellationToken cancellationToken) {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
