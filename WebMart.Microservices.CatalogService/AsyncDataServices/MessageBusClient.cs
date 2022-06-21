﻿using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using WebMart.Microservices.Extensions.DTOs.Product;

namespace WebMart.Microservices.CatalogService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration config)
        {
            _config = config;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_config["RabbitMQHost"]),
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> Connected to the Message Bus");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bu: {ex.Message}");
            }
        }

        public void PublishNewProduct(ProductPublishedDto productPublishedDto)
        {
            var message = JsonSerializer.Serialize(productPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection is open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is closed, sending canceled...");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("--> Message Bus disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection shutdown");
        }
    }
}
