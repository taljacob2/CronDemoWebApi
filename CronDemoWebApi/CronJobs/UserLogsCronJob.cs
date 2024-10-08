namespace CronDemoWebApi.CronJobs
{
    public class UserLogsCronJob : BackgroundService
    {
        private readonly ILogger<UserLogsCronJob> _logger;

        public UserLogsCronJob(ILogger<UserLogsCronJob> logger)
        {
            _logger = logger;
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
            await CronJobsBuilder.CreateCronJobAsync(
                "* * * * *", RunAllUsers, stoppingToken);
        }
    }
}
