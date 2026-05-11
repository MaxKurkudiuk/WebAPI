using WebAPI.Application.Interfaces;
using WebAPI.Domain.Jobs;

namespace WebAPI.Application.Jobs; 

public sealed class JobProcessor(IJobQueue jobQueue, IJobStore jobStore, ILogger<JobProcessor> logger) : BackgroundService {
    private readonly IJobQueue _jobQueue = jobQueue;
    private readonly IJobStore _jobStore = jobStore;
    private readonly ILogger<JobProcessor> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Job processor started.");

        while (!stoppingToken.IsCancellationRequested) {

            Job job;

            try {
                job = await _jobQueue.DequeueAsync(stoppingToken);
            } catch (OperationCanceledException) {
                // App is shutting down
                break;
            }

            using (_logger.BeginScope(new Dictionary<string, object> {
                ["JobId"] = job.Id,
                ["CorrelationId"] = job.CorrelationId ?? "N/A"
            })) {
                try {
                    job.State = JobState.Running;
                    job.StartedAt = DateTimeOffset.UtcNow;
                    _jobStore.Update(job);
                    _logger.LogInformation("Processing job {JobType}", job.JobType);

                    await ProcessJobAsync(job, stoppingToken);

                    job.State = JobState.Completed;
                    job.FinishedAt = DateTimeOffset.UtcNow;
                    _jobStore.Update(job);
                    _logger.LogInformation("Job completed successfully.");
                } catch (Exception ex) {
                    job.State = JobState.Failed;
                    job.FinishedAt = DateTimeOffset.UtcNow;
                    _jobStore.Update(job);
                    _logger.LogError(ex, "Job entered state {JobState}", job.State);
                }
            }
        }

        _logger.LogInformation("JobProcessor stopped.");
    }

    private static async Task ProcessJobAsync(Job job, CancellationToken token) {
        // Simulate real work (Excel, Graph, APIs, etc.)
        await Task.Delay(TimeSpan.FromSeconds(2), token);
    }
}
