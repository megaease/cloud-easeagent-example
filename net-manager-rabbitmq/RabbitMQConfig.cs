using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.manager.rabbitmq
{
    public class RabbitMQConfig
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Queue { get; set; }

        public RabbitMQConfig()
        {
            string? host = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
            string? portStr = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
            string? queue = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE");
            if (host == null || host.Trim().Equals(""))
            {
                host = "127.0.0.1";
            }
            int port = 5672;
            if (portStr != null)
            {
                port = int.Parse(portStr);
            }
            if (queue == null || queue.Trim().Equals(""))
            {
                queue = "user_manager";
            }
            this.HostName = host;
            this.Port = port;
            this.Queue = queue;
            Console.WriteLine($"create RabbitMQConfig by host:{host} port:{port} queue:{queue}");
        }
    }
}