using System;

namespace task18
{
    public interface IYieldableCommand : ICommand
    {
        bool IsCompleted { get; }
    }
}