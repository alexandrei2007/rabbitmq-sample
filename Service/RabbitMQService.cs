using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace Service
{
    public class RabbitMQService
    {
        public const string EXCHANGE_NAME = "exchange1";
        public const string ROUTING_KEY = "key1";
        public const string QUEUE_NAME = "queue1";

        public void SendMessage(string message)
        {
            using (var conn = ConnectionManager.CreateConnection())
            {
                using (var model = conn.CreateModel())
                {
                    var properties = model.CreateBasicProperties();
                    properties.ContentType = "text/plain";
                    properties.DeliveryMode = 2; // persitent

                    var body = System.Text.Encoding.UTF8.GetBytes(message);

                    model.BasicPublish(exchange: EXCHANGE_NAME,
                                       routingKey: ROUTING_KEY,
                                       basicProperties: properties,
                                       body: body);
                }
            }
        }

        public IDisposable ConsumeMessages(string name = null)
        {
            var conn = ConnectionManager.CreateConnection();
            var model = conn.CreateModel();

            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (channel, deliverEventArgs) =>
            {
                var body = System.Text.Encoding.UTF8.GetString(deliverEventArgs.Body);
                Console.WriteLine($"[{name}] {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {body.ToString()}");

                model.BasicAck(deliverEventArgs.DeliveryTag, false);
            };

            model.BasicConsume(QUEUE_NAME, false, consumer);

            return conn;
        }

        public void EnsureResourceCreation()
        {
            using (var conn = ConnectionManager.CreateConnection())
            {
                using (var model = conn.CreateModel())
                {
                    model.ExchangeDeclare(
                        exchange: EXCHANGE_NAME,
                        type: "direct",
                        durable: true,
                        autoDelete: false,
                        arguments: null);

                    model.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    model.QueueBind(
                        queue: QUEUE_NAME,
                        exchange: EXCHANGE_NAME,
                        routingKey: ROUTING_KEY);
                }
            }
        }

    }
}
