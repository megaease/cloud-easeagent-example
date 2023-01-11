using RabbitMQ.Client;
using System.Text;

namespace net.manager.rabbitmq
{
    public class Send
    {
        private ConnectionFactory factory;
        private string queue;


        public Send()
        {
            RabbitMQConfig config = new RabbitMQConfig();
            factory = new ConnectionFactory()
            {
                HostName = config.HostName,
                Port = config.Port
            };
            this.queue = config.Queue;
        }

        public void Publish(string data)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(data);

                channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", data);
            }
        }
    }
}