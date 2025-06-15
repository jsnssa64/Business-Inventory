
using Confluent.Kafka;
using System.Threading;

namespace NotificationService.KafkaConsumerService
{
    public class KafkaConsumerService : BackgroundService
    {
        public KafkaConsumerService()
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "host1:9092,host2:9092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topics);

                while (!cancelled)
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                }

                consumer.Close();
            }
        }
    }
}
