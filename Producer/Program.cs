using Service;
using System;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new RabbitMQService();
            service.EnsureResourceCreation();

            Console.WriteLine("Type your message or (q) to quit:");

            while (true)
            {
                var message = Console.ReadLine();

                if (message == "q")
                    return;

                if (!string.IsNullOrEmpty(message))
                {
                    try
                    {
                        service.SendMessage(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error]: Failed to send message {ex.Message}");
                    }
                }
            }

        }
    }
}
