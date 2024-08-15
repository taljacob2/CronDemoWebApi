using Hangfire;
using Hangfire.Console;
using Hangfire.Server;

namespace CronDemoWebApi
{
    public class CronJobs
    {
        public static void RunAllUsers(PerformContext context)
        {
            context.WriteLine("Recurring job for all users!");
            var db = new DB();

            var progress = context.WriteProgressBar();
            var currentSuccessfulTasks = 0;

            Parallel.ForEach(db.Users, user =>
            {
                RunUser(user, context);

                // Update the successful task count
                Interlocked.Increment(ref currentSuccessfulTasks);

                // Update progress bar
                progress.SetValue((currentSuccessfulTasks / db.Users.Count()) * 100);
            });
        }

        public static void RunUser(string user, PerformContext context)
        {
            context.WriteLine($"Hello {user}");
            for (int i = 0; i < 10; i++)
            {
                context.WriteLine(user);
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
                    () => CronJobs.RunAllUsers(null),
                    "* * * * *", // Cron expression for every minute
                    new RecurringJobOptions() { TimeZone = TimeZoneInfo.Utc });
            }
        }
    }
}
