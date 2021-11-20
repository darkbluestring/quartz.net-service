using Microsoft.AspNetCore.Mvc;
using Quartz.WebApi.Data;

namespace Quartz.WebApi.Rest.Controllers;

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
