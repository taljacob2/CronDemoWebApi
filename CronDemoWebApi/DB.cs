namespace CronDemoWebApi
{
    public class DB
    {
        public IList<string> Users { get; private set; } = new List<string>();

        public DB()
        {
            for (int i = 0; i < 10; i++)
            {
                Users.Add(Guid.NewGuid().ToString());
            }
        }
    }
}
