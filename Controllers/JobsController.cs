using Microsoft.AspNetCore.Mvc;
using WebAPI_01.Application.Interfaces;
using WebAPI_01.Domain.Jobs;
using WebAPI_01.Dtos;

namespace WebAPI_01.Controllers;

[ApiController]
[Route("jobs")]
public sealed class JobsController : ControllerBase {
    private readonly IJobQueue _queue;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobQueue queue, ILogger<JobsController> logger) {
        _queue = queue;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<CreateJobResponse> CreateJob(
        [FromBody] CreateJobRequest request) {
        var job = new Job {
            JobType = request.JobType,
            Parameters = request.Parameters
        };

        _queue.Enqueue(job);

        _logger.LogInformation("Job {JobId} enqueued.", job.Id);

        return Accepted(new CreateJobResponse {
            JobId = job.Id,
            Status = "Queued"
        });
    }
}

