using NCrontab;

namespace CronDemoWebApi.Services
{
    public class CronJobsService
    {
        public async Task CreateCronJobAsync(string cronExpression, Action action)
        {
            // Create a CrontabSchedule instance based on the cron expression
            var schedule = CrontabSchedule.Parse(cronExpression);
            var nextOccurrence = schedule.GetNextOccurrence(DateTime.UtcNow);

            // Create a CancellationTokenSource to control the application's runtime
            using (var cts = new CancellationTokenSource())
            {
                var token = cts.Token;

                while (!token.IsCancellationRequested)
                {
                    // Wait until the next occurrence
                    var delay = nextOccurrence - DateTime.UtcNow;
                    if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

                    await Task.Delay(delay, token);

                    // Invoke action
                    action.Invoke();

                    // Calculate the next occurrence
                    nextOccurrence = schedule.GetNextOccurrence(DateTime.UtcNow);
                }
            }
        }
    }
}