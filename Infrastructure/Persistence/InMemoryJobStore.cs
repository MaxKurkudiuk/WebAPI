using System.Collections.Concurrent;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Jobs;

namespace WebAPI.Infrastructure.Persistence;

public sealed class InMemoryJobStore : IJobStore {
    private readonly ConcurrentDictionary<Guid, Job> _jobs = new();

    public void Add(Job job) {
        _jobs[job.Id] = job;
    }

    public Job? Get(Guid jobId) {
        _jobs.TryGetValue(jobId, out var job);
        return job;
    }

    public void Update(Job job) {
        _jobs[job.Id] = job;
    }
}

