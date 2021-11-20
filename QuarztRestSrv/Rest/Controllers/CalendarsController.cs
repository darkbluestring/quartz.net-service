using Microsoft.AspNetCore.Mvc;

namespace Quartz.WebApi.Rest.Controllers;

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