using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using System.Collections.ObjectModel;
using Quartz;
using Quartz.WebApi.Data;

namespace Quartz.WebApi.Rest.Controllers;


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
