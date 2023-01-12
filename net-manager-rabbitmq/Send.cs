using easeagent;
using RabbitMQ.Client;
using System.Text;
using zipkin4net;
using zipkin4net.Propagation;

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

            Trace trace = Trace.Current.Child();
            trace.Record(Annotations.ProducerStart());
            trace.Record(Annotations.Rpc(queue));
            Agent.RecordMiddleware(trace, easeagent.Middleware.Type.RabbitMQ);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(data);

                var props = channel.CreateBasicProperties();
                props.Headers = new Dictionary<string, Object>();
                foreach (var item in Agent.InjectToDict(trace))
                {
                    props.Headers.Add(item.Key, item.Value);
                }

                channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: props,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", data);
            }
            trace.Record(Annotations.ProducerStop());
        }
    }
}