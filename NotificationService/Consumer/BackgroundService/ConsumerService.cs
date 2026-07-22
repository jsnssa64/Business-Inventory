using Confluent.Kafka;
using System.Threading;

namespace NotificationService.Consumer.ConsumerService
{
    public class ConsumerService : BackgroundService
    {
        private volatile bool _cancelled; // Added field to track cancellation  

        public ConsumerService()
        {
            _cancelled = false; // Initialize the cancellation flag  
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "host1:9092,host2:9092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var topics = new List<string>();

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topics);

                while (!_cancelled && !stoppingToken.IsCancellationRequested) // Fixed condition  
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                }
                consumer.Close();
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _cancelled = true; // Set cancellation flag when stopping  
            return base.StopAsync(cancellationToken);
        }
    }
}
