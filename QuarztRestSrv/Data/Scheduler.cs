using Quartz;

namespace Quartz.RestApi.Data;

public class SchedulerState
{

    public SchedulerState(IScheduler scheduler)
    {

        InStandbyMode = scheduler.InStandbyMode;
        IsShutdown = scheduler.IsShutdown;
        IsStarted = scheduler.IsStarted;
        SchedulerInstanceId = scheduler.SchedulerInstanceId;
        SchedulerName = scheduler.SchedulerName;
        //MetaData = await scheduler.GetMetaData()
        // TODO: add other meta data methods
    }

    public bool InStandbyMode { get; set; }
    public bool IsShutdown { get; set; }
    public bool IsStarted { get; set; }
    public string SchedulerInstanceId { get; set; } = "";
    public string SchedulerName { get; set; } = "";
    //public  SchedulerMetaData? MetaData {get;set;}
}

public class JobDetail
{

    public JobDetail(IJobDetail job)
    {
        Key = job.Key.ToString();
        Description = job.Description;
        JobType = job.JobType.ToString();
        JobDataMap = string.Join(",", job.JobDataMap);
        Durable = job.Durable;
        PersistJobDataAfterExecution = job.PersistJobDataAfterExecution;
        ConcurrentExecutionDisallowed = job.ConcurrentExecutionDisallowed;
        RequestsRecovery = job.RequestsRecovery;
    }
    /// <summary>
    /// The key that identifies this jobs uniquely.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Get or set the description given to the <see cref="IJob" /> instance by its
    /// creator (if any).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Get or sets the instance of <see cref="IJob" /> that will be executed.
    /// </summary>
    public string? JobType { get; set; }

    /// <summary>
    /// Get or set the <see cref="JobDataMap" /> that is associated with the <see cref="IJob" />.
    /// </summary>
    public string? JobDataMap { get; set; }

    /// <summary>
    /// Whether or not the <see cref="IJob" /> should remain stored after it is
    /// orphaned (no <see cref="ITrigger" />s point to it).
    /// </summary>
    /// <remarks>
    /// If not explicitly set, the default value is <see langword="false" />.
    /// </remarks>
    /// <returns> 
    /// <see langword="true" /> if the Job should remain persisted after being orphaned.
    /// </returns>
    public bool Durable { get; set; }

    /// <summary>
    /// Whether the associated Job class carries the <see cref="PersistJobDataAfterExecutionAttribute" />.
    /// </summary>
    /// <seealso cref="PersistJobDataAfterExecutionAttribute" />
    public bool PersistJobDataAfterExecution { get; set; }

    /// <summary>
    /// Whether the associated Job class carries the <see cref="DisallowConcurrentExecutionAttribute" />.
    /// </summary>
    /// <seealso cref="DisallowConcurrentExecutionAttribute"/>
    public bool ConcurrentExecutionDisallowed { get; set; }

    /// <summary>
    /// Set whether or not the <see cref="IScheduler" /> should re-Execute
    /// the <see cref="IJob" /> if a 'recovery' or 'fail-over' situation is
    /// encountered.
    /// </summary>
    /// <remarks>
    /// If not explicitly set, the default value is <see langword="false" />.
    /// </remarks>
    /// <seealso cref="IJobExecutionContext.Recovering" />
    public bool RequestsRecovery { get; set; }

}