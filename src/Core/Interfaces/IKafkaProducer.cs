namespace Infraestrutura.Kafka
{
    public interface IKafkaProducer
    {
        Task PublicarAsync<T>(string topico, T mensagem);
    }
}
