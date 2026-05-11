using WebAPI.Application.Interfaces;
using WebAPI.Domain.Jobs;

namespace WebAPI.Application.Jobs; 

public sealed class JobProcessor : BackgroundService {
    private readonly IJobQueue _jobQueue;
    private readonly ILogger<JobProcessor> _logger;

    public JobProcessor(IJobQueue jobQueue, ILogger<JobProcessor> logger) {
        _jobQueue = jobQueue;
        _logger = logger;
    }

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
                    job.Status = "Running";
                    _logger.LogInformation("Processing job {JobType}", job.JobType);

                    await ProcessJobAsync(job, stoppingToken);

                    job.Status = "Completed";
                    _logger.LogInformation("Job completed successfully.");
                } catch (Exception ex) {
                    job.Status = "Failed";
                    _logger.LogError(ex, "Job processing failed.");
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
