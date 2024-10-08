using NCrontab;

namespace CronDemoWebApi.Utils
{
    public class CronJobsBuilder
    {
        public async Task CreateCronJobAsync(
            string cronExpression, Action action, CancellationToken stoppingToken)
        {
            // Create a CrontabSchedule instance based on the cron expression
            var schedule = CrontabSchedule.Parse(cronExpression);
            var nextOccurrence = schedule.GetNextOccurrence(DateTime.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait until the next occurrence
                var delay = nextOccurrence - DateTime.UtcNow;
                if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

                await Task.Delay(delay, stoppingToken);

                // Invoke action
                action.Invoke();

                // Calculate the next occurrence
                nextOccurrence = schedule.GetNextOccurrence(DateTime.UtcNow);
            }
        }
    }
}