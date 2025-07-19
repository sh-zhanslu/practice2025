using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using task18;

namespace task18tests
{
    public class TestActionCommand : ICommand
    {
        private readonly Action _action;
        
        public TestActionCommand(Action action)
        {
            _action = action;
        }
        
        public void Execute() => _action?.Invoke();
    }

    public class CommandServerTests
    {
        [Fact]
        public void LongRunningCommand_CompletesInMultipleSteps()
        {
            var scheduler = new RoundRobinScheduler();
            using var server = new CommandServer(scheduler);
            var longCmd = new LongRunningCommand(3);
            
            server.Start();
            server.AddCommand(longCmd);
            Thread.Sleep(400);
            
            Assert.True(longCmd.IsCompleted);
        }

        [Fact]
        public void MixedCommands_ExecuteInRoundRobinFashion()
        {
            var scheduler = new RoundRobinScheduler();
            using var server = new CommandServer(scheduler);
            var results = new List<string>();
            var cmd1 = new LongRunningCommand(2);
            var cmd2 = new TestActionCommand(() => results.Add("Instant")); 
            
            server.Start();
            server.AddCommand(cmd1);
            server.AddCommand(cmd2);
            Thread.Sleep(300);
            
            Assert.Contains("Instant", results);
            Assert.True(cmd1.IsCompleted);
        }

        [Fact]
        public void Scheduler_PreservesExecutionOrder()
        {
            var scheduler = new RoundRobinScheduler();
            using var server = new CommandServer(scheduler);
            var commands = Enumerable.Range(1, 5)
                .Select(i => new LongRunningCommand(2))
                .ToList();
            
            server.Start();
            foreach (var cmd in commands)
            {
                server.AddCommand(cmd);
            }
            Thread.Sleep(1000);
            
            Assert.All(commands, c => Assert.True(c.IsCompleted));
        }
    }
}