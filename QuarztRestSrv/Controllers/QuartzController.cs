using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using Quartz;
using QuarztRestSrv.Data;
using Quartz.Impl.Matchers;

namespace QuarztRestSrv.Controllers;


public abstract class  BaseQuartzController : ControllerBase
{
    protected readonly ISchedulerFactory _schedulerFactory;
    protected BaseQuartzController( ISchedulerFactory scheduler)
    {
        _schedulerFactory=scheduler;
    }

    protected async Task<IScheduler> GetScheduler() {
        var scheduler = await _schedulerFactory.GetScheduler();

        return scheduler;
    }
}


[ApiController]
[Route("[controller]")]
public class QuartzController : BaseQuartzController
{
    private readonly ILogger<QuartzController> _logger;
    //private readonly ISchedulerFactory _schedulerFactory;
    public QuartzController(
        ILogger<QuartzController> logger, 
        ISchedulerFactory scheduler):base(scheduler)
    {
        _logger = logger;
       
    }

    [HttpGet(Name = "GetSchedulerInfo")]
    public async Task<SchedulerState> GetSchedulerInfo()
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        return new SchedulerState(scheduler) ;
    }


}

[ApiController]
[Route("[controller]")]
public class JobController : BaseQuartzController
{

   private readonly ILogger<QuartzController> _logger;
    //private readonly ISchedulerFactory _schedulerFactory;
    public JobController(
        ILogger<QuartzController> logger,
        ISchedulerFactory scheduler):base(scheduler)
    {
        _logger = logger;
        //_schedulerFactory=scheduler;
    }

    
    public async Task<IReadOnlyCollection<string>> GetJobGroupNames() {
        
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobGroupNames =  await scheduler.GetJobGroupNames();

        return new ReadOnlyCollection<String>(jobGroupNames.ToList<String>());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDetail>>> Get() {
        var jobs = new List<JobDetail>();

        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

        foreach ( JobKey jobKey in jobKeys) {
            var jobDetail = await scheduler.GetJobDetail(jobKey);
            if (jobDetail != null) {
                jobs.Add(new JobDetail(jobDetail));
            }
        }
    
        return jobs;
    }

    [HttpGet("{jobKey}",Name = "GetJobDetail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetail>> GetJobDetail(string jobKey)
    {
        IScheduler scheduler = await _schedulerFactory.GetScheduler();

        var jobDetail = await scheduler.GetJobDetail(new JobKey(jobKey));

        if (jobDetail == null) return this.NotFound();
        
        return Ok(new JobDetail(jobDetail));
    }
}