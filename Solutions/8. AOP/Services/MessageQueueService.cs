using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace WindowsService
{
    public interface IMessageQueueSenderService
    {
        void Send(string message);
    }

    public interface IMessageQueueReceiverService
    {
        string Receive();
    }

    public class MessageQueueService : IMessageQueueSenderService, IMessageQueueReceiverService
    {
        private const string DefaultHostName = "localhost";
        private const string DefaultQueueName = "concatenation";

        private readonly string _queueName;
        private readonly ConnectionFactory _factory;

        public MessageQueueService(string hostName = null, string queueName = null)
        {
            _queueName = queueName ?? DefaultQueueName;
            _factory = new ConnectionFactory() { HostName = hostName ?? DefaultHostName };
        }

        public void Send(string message)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: _queueName,
                                     basicProperties: null,
                                     body: body);
            }
        }

        public string Receive()
        {
            string message = null;
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                };
                channel.BasicConsume(queue: _queueName,
                                     noAck: true,
                                     consumer: consumer);

                while (message == null) ;
            }

            return message;
        }
    }
}
