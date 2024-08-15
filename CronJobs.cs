using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using System.Collections.Concurrent;

namespace CronDemoWebApi
{
    public class CronJobs
    {
        public static void RunAllUsers(PerformContext context)
        {
            context.WriteLine("Recurring job for all users!");
            var db = new DB();

            var progress = context.WriteProgressBar();
            var userCount = db.Users.Count();
            var successfulTasks = new ConcurrentBag<bool>(); // Thread-safe collection to count successful tasks

            Parallel.ForEach(db.Users, user =>
            {
                try
                {
                    RunUser(user, context);
                    successfulTasks.Add(true); // Add to the successful tasks
                }
                catch
                {
                    // Handle or log exceptions as needed
                }
                finally
                {
                    // Update the progress bar safely
                    var completedCount = successfulTasks.Count;
                    var progressValue = (completedCount * 100) / userCount;
                    progress.SetValue(progressValue);
                }
            });
        }

        public static void RunUser(string user, PerformContext context)
        {
            context.WriteLine($"Hello {user}");
            for (int i = 0; i < 10; i++)
            {
                context.WriteLine(user);
                Thread.Sleep(1000); // Simulate work
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