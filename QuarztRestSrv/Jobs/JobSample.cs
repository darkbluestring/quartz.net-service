using System;
using Quartz;
namespace QuarztRestSrv.Jobs
{
    public class JobSample : IJob
    {
        public JobSample()
        {
        }

        public virtual Task Execute(IJobExecutionContext context)
        {
            // Say Hello to the World and display the date/time
            var timestamp = DateTime.Now;
            Console.WriteLine($"Hello World! - {timestamp:yyyy-MM-dd HH:mm:ss.fff}");
            return Task.CompletedTask;
        }
    }
}
