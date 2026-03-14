using Confluent.Kafka;
using System.Text.Json;

namespace Infraestrutura.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(string bootstrapServers)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublicarAsync<T>(string topico, T mensagem)
        {
            var json = JsonSerializer.Serialize(mensagem);

            await _producer.ProduceAsync(
                topico,
                new Message<Null, string>
                {
                    Value = json
                });
        }
    }
}
