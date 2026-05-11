using Microsoft.AspNetCore.Mvc;
using WebAPI.Application.Interfaces;
using WebAPI.Domain.Jobs;
using WebAPI.Dtos;

namespace WebAPI.Controllers;

[ApiController]
[Route("jobs")]
public sealed class JobsController(IJobQueue queue, IJobStore store, ILogger<JobsController> logger) : ControllerBase {
    private const string CorrelationHeader = "X-Correlation-Id";

    private readonly IJobQueue _queue = queue;
    private readonly IJobStore _store = store;
    private readonly ILogger<JobsController> _logger = logger;

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

        _store.Add(job);
        _queue.Enqueue(job);

        _logger.LogInformation("Job {JobId} created", job.Id);

        return Accepted(new CreateJobResponse {
            JobId = job.Id,
            State = job.State
        });
    }

    [HttpGet("{id:guid}")]
    public ActionResult<JobStatusResponse> GetJobStatus(Guid id) {
        var job = _store.Get(id);

        if (job is null)
            return NotFound();

        return Ok(new JobStatusResponse {
            JobId= job.Id,
            State= job.State,
            CreatedAt = job.CreatedAt,
            StartedAt = job.StartedAt,
            FinishedAt = job.FinishedAt
        });
    }
}
