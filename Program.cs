using CronDemoWebApi;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configure Hangfire to use in-memory storage for testing
GlobalConfiguration.Configuration.UseMemoryStorage();

// Create and start a new Hangfire server
using (var server = new BackgroundJobServer())
{
    // Enqueue a job
    //BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget!"));

    RecurringJob.AddOrUpdate(
        $"RecurringJobForAllUsers",
        () => CronJobs.RunAllUsers(),
        "* * * * *", // Cron.Monthly
        new RecurringJobOptions() { TimeZone = TimeZoneInfo.Utc });

    // Keep the application running to process background jobs
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}

app.Run();
