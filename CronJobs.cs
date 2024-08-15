using Hangfire;

namespace CronDemoWebApi
{
    public class CronJobs
    {
        public static void RunAllUsers()
        {
            Console.WriteLine("Recurring job for all users!");

            var db = new DB();
            Parallel.ForEach(db.Users, (user) => RunUser(user));
        }

        public static void RunUser(string user)
        {

            Console.WriteLine($"Hello {user}");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(user);
                Thread.Sleep(1000);
            }
        }

        public static void InitializeHangfireJobs(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate(
                    "RecurringJobForAllUsers",
                    () => CronJobs.RunAllUsers(),
                    "* * * * *", // Cron expression for every minute
                    new RecurringJobOptions() { TimeZone = TimeZoneInfo.Utc });
            }
        }
    }
}
