using Confluent.Kafka;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisBackgroundService
{
    public class Worker : BackgroundService
    {
        private readonly IDistributedCache? _cache;

        public Worker(ILogger<Worker> logger, IDistributedCache? cache)
        {
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092",
                ClientId = "myconsumerclient",
                GroupId = "MyConsumerGroup",
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            using var consumer = new ConsumerBuilder<int, int>(consumerConfig).Build();
            consumer.Subscribe("redis-remove");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumerData = consumer.Consume(TimeSpan.FromSeconds(3));
                if (consumerData is not null)
                {
                    var key = $"user-{consumerData.Message.Value}";
                    await _cache!.RemoveAsync(key);
                }
            }
        }
    }
}
