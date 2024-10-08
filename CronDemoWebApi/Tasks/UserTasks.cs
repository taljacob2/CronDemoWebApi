using CronDemoWebApi.Services;
using System.Collections.Concurrent;

namespace CronDemoWebApi.Tasks
{
    public class UserTasks
    {
        private readonly ILogger<UserTasks> _logger;
        private readonly CronJobsService _cronJobsService;

        public UserTasks(ILogger<UserTasks> logger, CronJobsService cronJobsService)
        {
            _logger = logger;
            _cronJobsService = cronJobsService;
        }

        public void Initialize()
        {
            _cronJobsService.CreateCronJobAsync("* * * * *", RunAllUsers).Wait();
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
    }
}
