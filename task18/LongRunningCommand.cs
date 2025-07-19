using System;

namespace task18
{
    public class LongRunningCommand : IYieldableCommand
    {
        public int RemainingSteps { get; private set; }
        public bool IsCompleted => RemainingSteps <= 0;
        
        public LongRunningCommand(int steps)
        {
            RemainingSteps = steps;
        }
        
        public void Execute()
        {
            if (IsCompleted) return;
            
            Thread.Sleep(50);
            RemainingSteps--;
        }
    }
}