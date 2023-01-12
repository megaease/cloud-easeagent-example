using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using zipkin4net.Propagation;
using zipkin4net;
using easeagent;

namespace net.manager.rabbitmq
{
    public class Receive
    {
        public static Receive RECEIVE { get; set; }
        public bool run = true;

        public String addEndpoint;
        public String deleteEndpoint;

        private IHttpClientFactory clientFactory;

        public Receive(IHttpClientFactory clientFactory)
        {
            string? endpoint = Environment.GetEnvironmentVariable("USER_ENDPOINT");
            if (endpoint == null)
            {
                this.addEndpoint = "http://127.0.0.1:18888/user/add/";
                this.deleteEndpoint = "http://127.0.0.1:18888/user/delete/";
            }
            else
            {
                this.addEndpoint = endpoint + "/add/";
                this.deleteEndpoint = endpoint + "/delete/";

            }
            this.clientFactory = clientFactory;
        }
        public void Start()
        {
            RabbitMQConfig config = new RabbitMQConfig();
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("start consumer.");
            var factory = config.CreateFactory();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: config.Queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    Trace trace = Agent.ExtractToTrace((key) =>
                    {
                        if (ea.BasicProperties.Headers.ContainsKey(key))
                        {
                            return System.Text.Encoding.Default.GetString((byte[])ea.BasicProperties.Headers[key]);
                        }
                        else
                        {
                            return null;
                        }
                    });
                    Agent.RecordMiddleware(trace, easeagent.Middleware.Type.RabbitMQ);
                    trace.Record(Annotations.ConsumerStart());
                    trace.Record(Annotations.Rpc(ea.RoutingKey));
                    Agent.Current(trace, () => consumerRow(message));
                    trace.Record(Annotations.ConsumerStop());
                };
                channel.BasicConsume(queue: config.Queue,
                                     autoAck: true,
                                     consumer: consumer);
                while (run)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public void consumerRow(string message)
        {
            string[] msg = message.Split(",", 2);
            if (msg[0].Equals("add"))
            {
                callAddAsync(msg[1]);
            }
            if (msg[0].Equals("delete"))
            {
                callDelete(msg[1]);
            }
        }

        private async void callAddAsync(string name)
        {
            await callGetAsync(addEndpoint + name);
        }

        private async void callDelete(string name)
        {
            await callGetAsync(deleteEndpoint + name);
        }

        private async Task callGetAsync(string url)
        {
            using (var httpClient = clientFactory.CreateClient("UserManager"))
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("call url({0}) fail {1}", url, response.ToString());
                    }
                    else
                    {
                        Console.WriteLine("call url({0}) success {1}", url, response.ToString());
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("call url({0}) error {1}", url, e.ToString());
                }
            }
        }


        public void Stop()
        {
            run = false;
        }
    }
}