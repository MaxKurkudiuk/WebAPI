using WebAPI_01.Application.Interfaces;
using WebAPI_01.Domain.Jobs;

namespace WebAPI_01.Application.Jobs; 

public sealed class JobProcessor : BackgroundService {
    private readonly IJobQueue _queue;
    private readonly ILogger<JobProcessor> _logger;

    public JobProcessor(IJobQueue queue, ILogger<JobProcessor> logger) {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Job processor started.");

        while (!stoppingToken.IsCancellationRequested) {
            var job = await _queue.DequeueAsync(stoppingToken);

            using (_logger.BeginScope(new Dictionary<string, object> {
                ["JobId"] = job.Id
            })) {
                try {
                    job.Status = "Running";
                    _logger.LogInformation("Processing job {JobType}", job.JobType);

                    // 🔹 Simulate real automation work
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                    job.Status = "Completed";
                    _logger.LogInformation("Job completed successfully.");
                } catch (Exception ex) {
                    job.Status = "Failed";
                    _logger.LogError(ex, "Job failed.");
                }
            }
        }
    }

    private static async Task ProcessJobAsync(Job job, CancellationToken token) {
        // Simulate real work (Excel, Graph, APIs, etc.)
        await Task.Delay(TimeSpan.FromSeconds(2), token);
    }
}
