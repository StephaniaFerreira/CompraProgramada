using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestrutura.Kafka
{
    public interface IKafkaProducer
    {
        Task PublicarAsync<T>(string topico, T mensagem);
    }
}
