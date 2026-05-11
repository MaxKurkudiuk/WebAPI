using Microsoft.AspNetCore.Mvc;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Jobs;
using WebAPI.Dtos;

namespace WebAPI.Controllers;

[ApiController]
[Route("jobs")]
public sealed class JobsController : ControllerBase {
    private const string CorrelationHeader = "X-Correlation-Id";

    private readonly IJobQueue _queue;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobQueue queue, ILogger<JobsController> logger) {
        _queue = queue;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<CreateJobResponse> CreateJob(
        [FromBody] CreateJobRequest request) {

        var correlationId = HttpContext.Items[CorrelationHeader]?.ToString();

        var job = new Job {
            JobType = request.JobType,
            Parameters = request.Parameters,
            CorrelationId = correlationId,
            State = JobState.Pending
        };

        _queue.Enqueue(job);

        _logger.LogInformation(
                    "Job {JobId} enqueued with state {State}",
                    job.Id,
                    job.State);

        return Accepted(new CreateJobResponse {
            JobId = job.Id,
            State = job.State
        });
    }
}
