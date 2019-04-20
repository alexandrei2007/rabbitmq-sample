using Service;
using System;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i <= 5; i++)
            {
                var counter = i;

                var thread = new System.Threading.Thread(() =>
                {
                    Consumer(counter);
                });

                thread.Start();
            }
        }

        static void Consumer(int n)
        {
            var thread = $"thread{n}";
            Console.WriteLine($"Waiting for messages on {thread}...");

            var service = new RabbitMQService();
            service.EnsureResourceCreation();

            using (var conn = service.ConsumeMessages($"{thread}"))
            {
                Console.ReadLine();
                Console.WriteLine($"Closing {thread}");
            }
        }
    }
}
