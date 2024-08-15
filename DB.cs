namespace CronDemoWebApi
{
    public class DB
    {
        public IEnumerable<string> Users { get; private set; } = new List<string>()
        {
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        };
    }
}
