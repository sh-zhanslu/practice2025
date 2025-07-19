using System;
using System.Collections.Generic;
using System.Threading;

namespace task19
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрация длительных операций и прерывания ===");
            Console.WriteLine("Создаем сервер команд с планировщиком...");
            
            var scheduler = new RoundRobinScheduler();  
            using var server = new CommandServer(scheduler);  
            var commands = new List<TestCommand>();

            for (int i = 0; i < 5; i++)
            {
                commands.Add(new TestCommand(i));
            }

            server.Start();
            foreach (var cmd in commands)
            {
                server.AddCommand(cmd);
            }

            Console.WriteLine("Ожидаем выполнения команд...");
            Thread.Sleep(500);
            
            Console.WriteLine("\nПрерываем выполнение команд...");
            foreach (var cmd in commands)
            {
                cmd.Interrupt();
            }
            
            Console.WriteLine("Выполняем HardStop сервера");
            server.HardStop();
            
            Console.WriteLine("\n=== Результаты выполнения ===");
            foreach (var cmd in commands)
            {
                Console.WriteLine($"Команда {cmd._id} выполнена {cmd._counter}/{TestCommand.MaxExecutions} раз");
            }
            
            Console.WriteLine("Демонстрация завершена. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}