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
    }
}
