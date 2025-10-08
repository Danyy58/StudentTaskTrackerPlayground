using Confluent.Kafka;
using UserDeletionBGService;


namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppDbContext _context;

        public Worker(ILogger<Worker> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092",
                ClientId = "concumerClient",
                GroupId = "ConsumerGroupId",
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            using var consumer = new ConsumerBuilder<int, int>(consumerConfig).Build();
            consumer.Subscribe("user-deleted");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumerData = consumer.Consume(TimeSpan.FromSeconds(3));
                if (consumerData is not null)
                {
                    var userId = consumerData.Message.Value;
                    var tasks = _context.Task.Where(t => t.AuthorID == userId).ToList();
                    _context.Task.RemoveRange(tasks);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
