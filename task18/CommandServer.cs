using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task18
{
    public class CommandServer : IDisposable
    {
        private readonly BlockingCollection<ICommand> _newCommands = new BlockingCollection<ICommand>();
        private readonly IScheduler _scheduler;
        private readonly Thread _workerThread;
        private volatile bool _isRunning;
        private readonly ManualResetEventSlim _wakeEvent = new ManualResetEventSlim(false);

        public CommandServer(IScheduler scheduler = null)
        {
            _scheduler = scheduler ?? new RoundRobinScheduler();
            _workerThread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = "CommandProcessorThread"
            };
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _workerThread.Start();
        }

        public void AddCommand(ICommand command)
        {
            _newCommands.Add(command);
            _wakeEvent.Set();
        }

        private void WorkerLoop()
        {
            while (_isRunning)
            {
                ICommand cmd = null;

                if (_scheduler.HasCommand())
                {
                    cmd = _scheduler.Select();
                }
                else if (_newCommands.TryTake(out cmd))
                {

                }

                else
                {
                    _wakeEvent.Reset();
                    if (_scheduler.HasCommand()) continue;
                    
                    _wakeEvent.Wait(100);
                    continue;
                }

                try
                {
                    cmd.Execute();
                    
                    if (cmd is IYieldableCommand yieldable && !yieldable.IsCompleted)
                    {
                        _scheduler.Add(cmd);
                        _wakeEvent.Set();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke(cmd, ex);
                }
            }
        }

        public void SoftStop()
        {
            _isRunning = false;
            _wakeEvent.Set();
        }

        public void HardStop()
        {
            _isRunning = false;
            _newCommands.CompleteAdding();
            _wakeEvent.Set();
            if (_workerThread.IsAlive)
            {
                _workerThread.Interrupt();
            }
        }

        public Action<ICommand, Exception> ExceptionHandler { get; set; }

        public void Dispose()
        {
            HardStop();
            _wakeEvent?.Dispose();
            _newCommands?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}