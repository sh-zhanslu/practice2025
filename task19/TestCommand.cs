using System;
using System.Threading;

namespace task19
{
    public class TestCommand : IYieldableCommand  
    {
        private readonly int _id;
        private int _counter;
        public const int MaxExecutions = 3;
        private volatile bool _isInterrupted;

        public TestCommand(int id)
        {
            _id = id;
        }

        public bool IsCompleted => _counter >= MaxExecutions || _isInterrupted;

        public void Interrupt() => _isInterrupted = true;

        public void Execute()
        {
            if (IsCompleted) return;
            
            _counter++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Команда {_id} - выполнение #{_counter}");
            
            Thread.Sleep(100);
        }
    }
}