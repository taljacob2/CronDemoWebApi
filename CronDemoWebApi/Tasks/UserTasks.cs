using CronDemoWebApi.Services;

namespace CronDemoWebApi.Tasks
{
    public class UserTasks : BackgroundService
    {
        private readonly ILogger<UserTasks> _logger;
        private readonly CronJobsService _cronJobsService;

        public UserTasks(ILogger<UserTasks> logger, CronJobsService cronJobsService)
        {
            _logger = logger;
            _cronJobsService = cronJobsService;
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
            await _cronJobsService.CreateCronJobAsync(
                "* * * * *", RunAllUsers, stoppingToken);
        }
    }
}
