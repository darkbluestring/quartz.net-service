using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.WebApi.Graphql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddRouting();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
 {
     c.SwaggerDoc("v1", new OpenApiInfo
     {
         Version = "v1",
         Title = "Quartz.Net REST API",
         Description = "A REST api for the quart.net scheduler",
         TermsOfService = new Uri("https://example.com/terms"),
         Contact = new OpenApiContact
         {
             Name = "Some One",
             Email = string.Empty,
             Url = new Uri("https://twitter.com/spboyer"),
         },
         License = new OpenApiLicense
         {
             Name = "Use under LICX",
             Url = new Uri("https://example.com/license"),
         }
     });
 });

builder.Services.AddQuartz(q =>
{
    // handy when part of cluster or you want to otherwise identify multiple schedulers
    q.SchedulerId = "Scheduler-Core";

    // you can control whether job interruption happens for running jobs when scheduler is shutting down
    q.InterruptJobsOnShutdown = true;

    // when QuartzHostedServiceOptions.WaitForJobsToComplete = true or scheduler.Shutdown(waitForJobsToComplete: true)
    q.InterruptJobsOnShutdownWithWait = true;

    // we can change from the default of 1
    q.MaxBatchSize = 5;

    // we take this from appsettings.json, just show it's possible
    // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

    // this is default configuration if you don't alter it
    q.UseMicrosoftDependencyInjectionJobFactory();

    // these are the defaults
    q.UseSimpleTypeLoader();
    q.UseInMemoryStore();
    q.UseDefaultThreadPool(maxConcurrency: 10);

    // add XML configuration and poll it for changes
    q.UseXmlSchedulingConfiguration(x =>
    {
        x.Files = new[] { "quartz_jobs.xml" };
        x.ScanInterval = TimeSpan.FromMinutes(1);
        x.FailOnFileNotFound = true;
        x.FailOnSchedulingError = true;
    });

    // convert time zones using converter that can handle Windows/Linux differences
    q.UseTimeZoneConverter();

    // add some listeners
    //q.AddSchedulerListener<SampleSchedulerListener>();
    //q.AddJobListener<SampleJobListener>(GroupMatcher<JobKey>.GroupEquals(jobKey.Group));
    //q.AddTriggerListener<SampleTriggerListener>();

    // example of persistent job store using JSON serializer as an example
    /*
    q.UsePersistentStore(s =>
    {
        s.UseProperties = true;
        s.RetryInterval = TimeSpan.FromSeconds(15);
        s.UseSqlServer(sqlServer =>
        {
            sqlServer.ConnectionString = "some connection string";
            // this is the default
            sqlServer.TablePrefix = "QRTZ_";
        });
        s.UseJsonSerializer();
        s.UseClustering(c =>
        {
            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
            c.CheckinInterval = TimeSpan.FromSeconds(10);
        });
    });
    */
});

// ASP.NET Core hosting
builder.Services.AddQuartzServer(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.UseHttpsRedirection();



app.MapControllers();

app.Run();
