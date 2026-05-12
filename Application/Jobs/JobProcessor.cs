using WebAPI.Application.Interfaces;
using WebAPI.Application.Jobs.Processing;
using WebAPI.Domain.Jobs;

namespace WebAPI.Application.Jobs; 

public sealed class JobProcessor(
        IJobQueue jobQueue, 
        IJobStore jobStore,
        IEnumerable<IFileProcessor> processors,
        ILogger<JobProcessor> logger
    ) : BackgroundService {
    private readonly IJobQueue _jobQueue = jobQueue;
    private readonly IJobStore _jobStore = jobStore;
    private readonly ILogger<JobProcessor> _logger = logger;
    private readonly IEnumerable<IFileProcessor> _processors = processors;

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

    private async Task ProcessJobAsync(Job job, CancellationToken stoppingToken) {

        var filePath = job.Parameters["filePath"];

        var processor = _processors
            .FirstOrDefault(p => p.CanProcess(filePath));

        if (processor == null) {
            throw new InvalidOperationException(
                $"No processor found for {filePath}");
        }

        _logger.LogInformation("Processing file {FilePath}", filePath);

        var result = await processor.ProcessAsync(
            filePath,
            stoppingToken);

        _logger.LogInformation("File processed: {@Result}", result);
    }
}
