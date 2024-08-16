using System.Collections.Concurrent;
using NCrontab;

namespace CronDemoWebApi
{
    public class CronJobs
    {
        private readonly ILogger<CronJobs> _logger;

        public CronJobs(ILogger<CronJobs> logger)
        {
            _logger = logger;
        }

        private void RunAllUsers()
        {
            _logger.LogInformation("Recurring job for all users!");
            var db = new DB();

            // TODO:
            //var progress = logger.WriteProgressBar();
            var userCount = db.Users.Count();
            var successfulTasks = new ConcurrentBag<bool>(); // Thread-safe collection to count successful tasks

            Parallel.ForEach(db.Users, user =>
            {
                try
                {
                    RunUser(user);
                    successfulTasks.Add(true); // Add to the successful tasks
                }
                catch (Exception ex)
                {
                    // Handle or log exceptions as needed
                    _logger.LogError(ex.ToString());
                }
                finally
                {
                    // Update the progress bar safely
                    var completedCount = successfulTasks.Count;
                    var progressValue = (completedCount * 100) / userCount;
                    // TODO:
                    //progress.SetValue(progressValue);
                }
            });
        }

        private void RunUser(string user)
        {
            _logger.LogInformation($"Hello {user}");
            for (int i = 0; i < 10; i++)
            {
                _logger.LogInformation(user);
                Thread.Sleep(1000); // Simulate work
            }
        }

        public async Task InitCronJobsAsync()
        {
            await CreateCronJobAsync("* * * * *", RunAllUsers);
        }

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