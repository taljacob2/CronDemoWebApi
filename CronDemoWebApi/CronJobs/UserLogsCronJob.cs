using CronDemoWebApi.Utils;

namespace CronDemoWebApi.CronJobs
{
    public class UserLogsCronJob : BackgroundService
    {
        private readonly ILogger<UserLogsCronJob> _logger;
        private readonly CronJobsBuilder _cronJobsBuilder;

        public UserLogsCronJob(
            ILogger<UserLogsCronJob> logger, CronJobsBuilder cronJobsBuilder)
        {
            _logger = logger;
            _cronJobsBuilder = cronJobsBuilder;
        }

        private void RunAllUsers()
        {
            _logger.LogInformation("Recurring job for all users!");
            var db = new DB();

            Parallel.ForEach(db.Users, RunUser);
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _cronJobsBuilder.CreateCronJobAsync(
                "* * * * *", RunAllUsers, stoppingToken);
        }
    }
}
