using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace net.manager.rabbitmq
{
    public class RabbitMQConfig
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }

        public RabbitMQConfig()
        {
            string? portStr = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
            int port = 5672;
            if (portStr != null)
            {
                port = int.Parse(portStr);
            }
            this.HostName = getEnv("RABBITMQ_HOST", "127.0.0.1");
            this.Port = port;
            this.Queue = getEnv("RABBITMQ_QUEUE", "user_manager");
            this.User = getEnv("RABBITMQ_USER", "manager");
            this.Password = getEnv("RABBITMQ_PASSWORD", "manager");
            Console.WriteLine($"create RabbitMQConfig by host:{this.HostName} port:{port} queue:{this.Queue}");
        }

        private string getEnv(string name, string defaultValue)
        {
            string? v = Environment.GetEnvironmentVariable(name);
            if (v == null || v.Trim().Equals(""))
            {
                return defaultValue;
            }
            return v;
        }

        public ConnectionFactory CreateFactory()
        {
            return new ConnectionFactory()
            {
                HostName = HostName,
                Port = Port,
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                UserName = User,
                Password = Password,
                VirtualHost = "/",
            };
        }
    }
}