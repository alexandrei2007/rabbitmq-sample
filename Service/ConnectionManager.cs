using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.IO;

namespace Service
{
    public class ConnectionManager
    {
        protected static Settings _settings = null;

        public static IConnection CreateConnection()
        {
            var settings = GetSettings();

            var factory = new ConnectionFactory
            {
                UserName = settings.UserName,
                Password = settings.Password,
                VirtualHost = settings.VirtualHost,
                HostName = settings.HostName,
                Port = settings.Port,
                AutomaticRecoveryEnabled = true
            };

            return factory.CreateConnection();
        }

        public static Settings GetSettings()
        {
            if (_settings != null)
                return _settings;

            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            _settings = new Settings();
            configuration.GetSection("rabbitmq").Bind(_settings);

            return _settings;
        }
    }
}
