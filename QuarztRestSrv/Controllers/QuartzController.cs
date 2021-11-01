using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using System.Collections.ObjectModel;
using Quartz;
using Quartz.RestApi.Data;
using Quartz.Impl.Matchers;

namespace Quartz.RestApi.Controllers;


[Route("[controller]")]
public class SchedulerController : ControllerBase
{
    private readonly ILogger<SchedulerController> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public SchedulerController(
        ILogger<SchedulerController> logger,
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

    [Route("status")]
    [HttpGet]
    public async Task<SchedulerState> GetSchedulerInfo()
    {
        var scheduler = await GetScheduler();

        return new SchedulerState(scheduler);
    }

    [Route("standby")]
    [HttpPost]
    public async Task<ActionResult> StandbyScheduler()
    {
        var scheduler = await GetScheduler();

        await scheduler.Standby();

        return Ok();
    }
    [Route("start")]
    [HttpPost]
    public async Task<ActionResult> StartScheduler()
    {
        var scheduler = await GetScheduler();

        await scheduler.Start();

        return Ok();
    }
    [Route("startDelayed")]
    [HttpPost]
    public async Task<ActionResult> StartDelayedScheduler(TimeSpan timespan)
    {
        var scheduler = await GetScheduler();

        await scheduler.StartDelayed(timespan);

        return Ok();
    }

    [Route("shutdown")]
    [HttpPost]
    public async Task<ActionResult> ShutdownScheduler()
    {
        var scheduler = await GetScheduler();

        await scheduler.Shutdown();

        return Ok();
    }

    [Route("clearAll")]
    [HttpPost]
    public async Task<ActionResult> ClearAllScheduleItems()
    {
        var scheduler = await GetScheduler();

        await scheduler.Clear();

        return Ok();
    }
}

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

[Route("[controller]")]
public class TriggersController : ControllerBase
{
    private readonly ILogger<TriggersController> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public TriggersController(
        ILogger<TriggersController> logger,
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

    [Route("")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Trigger>> GetTriggers()
    {
        return Ok(new List<Trigger>());
    }

    [Route("{triggerId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Trigger> GetTrigger(string triggerId)
    {
        return Ok(new Trigger());
    }


}

[Route("[controller]")]
public class CalendarsController : ControllerBase
{
    private readonly ILogger<CalendarsController> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public CalendarsController(
        ILogger<CalendarsController> logger,
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

    [Route("")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetCalendarNames()
    {
        var scheduler = await GetScheduler();

        var names = await scheduler.GetCalendarNames();

        return Ok(new List<string>(names));
    }

    [Route("{name}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetCalendar(string name)
    {
        var scheduler = await GetScheduler();

        var calendar = await scheduler.GetCalendar(name);

        if (calendar == null)
        {
            return NotFound();
        }
        return Ok(calendar.Description);
    }
}