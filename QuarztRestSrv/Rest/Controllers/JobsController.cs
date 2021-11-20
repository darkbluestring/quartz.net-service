using Microsoft.AspNetCore.Mvc;
using Quartz.WebApi.Data;
using Quartz.Impl.Matchers;

namespace Quartz.WebApi.Rest.Controllers;

[Route("[controller]")]
public class JobsController : ControllerBase
{
    private readonly ILogger<JobsController> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public JobsController(
        ILogger<JobsController> logger,
        ISchedulerFactory scheduler)
    {
        _logger = logger;
        _schedulerFactory = scheduler;
    }

    private async Task<IScheduler> GetScheduler()
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        return scheduler;
    }


    [Route("pauseAll")]
    [HttpPost]
    public async Task<ActionResult> PauseAllJobs()
    {
        var scheduler = await GetScheduler();

        await scheduler.PauseAll();

        return Ok();
    }

    [Route("ResumeAll")]
    [HttpPost]
    public async Task<ActionResult> ResumeAllJobs()
    {
        var scheduler = await GetScheduler();

        await scheduler.ResumeAll();

        return Ok();
    }

    [Route("")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDetail>>> GetJobs()
    {
        var jobs = new List<JobDetail>();

        var scheduler = await GetScheduler();

        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

        foreach (JobKey jobKey in jobKeys)
        {
            var jobDetail = await scheduler.GetJobDetail(jobKey);
            if (jobDetail != null)
            {
                jobs.Add(new JobDetail(jobDetail));
            }
        }

        return jobs;
    }

    [Route("{jobKey}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetail>> GetJobDetail(string jobKey)
    {
        string[] jobKeyParts = jobKey.Split(".");
        IScheduler scheduler = await GetScheduler();

        IJobDetail? jobDetail = null;
        JobKey? key = null;
        switch (jobKeyParts.Length)
        {
            case 1:
                key = new JobKey(jobKeyParts[0]);
                break;
            case 2:
                key = new JobKey(jobKeyParts[1], jobKeyParts[0]);
                break;
        }

        if (key != null)
        {
            jobDetail = await scheduler.GetJobDetail(key);
        }

        if (jobDetail == null) return this.NotFound();

        return Ok(new JobDetail(jobDetail));
    }

    [Route("{jobKey}/triggers")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<Trigger>> GetJobTriggers(string jobKey)
    {
        return Ok(new List<Trigger>());
    }

    [Route("{jobKey}/triggers/{triggerId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Trigger> GetJobTrigger(string jobKey, string triggerId)
    {
        return Ok(new Trigger());
    }


}
