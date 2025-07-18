using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using task17;

namespace task17tests
{
    public class ServerThreadTests
    {
        [Fact]
        public void Commands_Execute_In_Order()
        {
            using var server = new ServerThread();
            var results = new List<int>();
            var sync = new object();

            server.Start();
            server.AddCommand(new ActionCommand(() => 
            {
                lock(sync) results.Add(1);
                Thread.Sleep(100);
            }));
            server.AddCommand(new ActionCommand(() => 
            {
                lock(sync) results.Add(2);
            }));

            Thread.Sleep(200);
            
            lock(sync)
            {
                Assert.Equal(new[] {1, 2}, results);
            }
        }

        [Fact]
        public void SoftStop_Completes_All_Commands()
        {
            using var server = new ServerThread();
            var counter = 0;
            
            server.Start();
            server.AddCommand(new ActionCommand(() => 
            {
                Thread.Sleep(50);
                Interlocked.Increment(ref counter);
            }));
            server.AddCommand(new ActionCommand(() => 
            {
                Thread.Sleep(50);
                Interlocked.Increment(ref counter);
            }));
            
            server.SoftStop();
            server.WorkerThread.Join();
            
            Assert.Equal(2, counter);
        }

        [Fact]
        public void HardStop_Stops_Immediately()
        {
            using var server = new ServerThread();
            var counter = 0;
            
            server.Start();
            server.AddCommand(new ActionCommand(() => 
            {
                Thread.Sleep(200);
                Interlocked.Increment(ref counter);
            }));
            server.AddCommand(new ActionCommand(() => 
            {
                Interlocked.Increment(ref counter);
            }));
            
            Thread.Sleep(50);
            server.HardStop();
            server.WorkerThread.Join(100);
            
            Assert.Equal(0, counter);
        }

        [Fact]
        public void HardStopCommand_Throws_When_Not_In_Target_Thread()
        {
            using var server = new ServerThread();
            var command = new HardStopCommand(server);
            
            var ex = Assert.Throws<InvalidOperationException>(command.Execute);
            Assert.Contains("должно выполняться в целевом потоке", ex.Message);
        }

        [Fact]
        public void SoftStopCommand_Throws_When_Not_In_Target_Thread()
        {
            using var server = new ServerThread();
            var command = new SoftStopCommand(server);
            
            var ex = Assert.Throws<InvalidOperationException>(command.Execute);
            Assert.Contains("должно выполняться в целевом потоке", ex.Message);
        }

        [Fact]
        public void ExceptionHandler_Receives_Exceptions()
        {
            using var server = new ServerThread();
            var receivedException = null as Exception;
            var testException = new Exception("Test error");
            
            server.ExceptionHandler = (cmd, ex) => receivedException = ex;
            server.Start();
            server.AddCommand(new ActionCommand(() => throw testException));
            
            Thread.Sleep(100);
            Assert.Same(testException, receivedException);
        }
    }
}