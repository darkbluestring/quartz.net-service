using System;
using Quartz.WebApi.Data;

namespace Quartz.WebApi.Graphql
{
    public class Query
    {
        private readonly ILogger<Query> _logger;
        private readonly ISchedulerFactory _schedulerFactory;

        public Query(
        ILogger<Query> logger,
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

        public async Task<SchedulerState> GetSchedulerStatus()
        {
            var scheduler = await GetScheduler();

            return new SchedulerState(scheduler);
        }
    }
}

