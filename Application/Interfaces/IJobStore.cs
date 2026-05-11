using WebAPI.Domain.Jobs;

namespace WebAPI.Application.Interfaces; 
public interface IJobStore {
    void Add(Job job);
    Job? Get(Guid jobId);
    void Update(Job job);
}
