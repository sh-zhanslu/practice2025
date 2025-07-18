using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ServerThread : IDisposable
{
    private readonly BlockingCollection<ICommand> _commandQueue = new BlockingCollection<ICommand>();
    private readonly Thread _workerThread;
    private volatile bool _isRunning;
    private volatile bool _softStopRequested;
    public Thread WorkerThread => _workerThread;

    public ServerThread()
    {
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
        _softStopRequested = false;
        _workerThread.Start();
    }

    public void AddCommand(ICommand command)
    {
        if (!_isRunning || _softStopRequested)
            throw new InvalidOperationException("Сервер не работает");
        
        _commandQueue.Add(command);
    }

    public void SoftStop()
    {
        if (!_isRunning) return;
        
        _softStopRequested = true;
        _commandQueue.Add(new ActionCommand(() => {}));
    }

    public void HardStop()
    {
        if (!_isRunning) return;
        
        _isRunning = false;
        _commandQueue.CompleteAdding();
        if (_workerThread.IsAlive)
        {
            _workerThread.Interrupt();
        }
    }

    private void WorkerLoop()
    {
        ICommand currentCommand = null;
        
        try
        {
            while (_isRunning)
            {
                try
                {
                    currentCommand = _commandQueue.Take();
                    currentCommand.Execute();
                    
                    if (_softStopRequested && _commandQueue.Count == 0)
                    {
                        _isRunning = false;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke(currentCommand, ex);
                }
                finally
                {
                    currentCommand = null;
                }
            }
        }
        finally
        {
            _commandQueue.Dispose();
        }
    }

    public Action<ICommand, Exception> ExceptionHandler { get; set; }

    public void Dispose()
    {
        HardStop();
        if (_workerThread.IsAlive)
        {
            _workerThread.Join(500);
        }
    }

    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        
        public ActionCommand(Action action)
        {
            _action = action;
        }
        
        public void Execute()
        {
            _action?.Invoke();
        }
    }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _targetServer;

    public HardStopCommand(ServerThread targetServer)
    {
        _targetServer = targetServer;
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _targetServer.WorkerThread)
            throw new InvalidOperationException("HardStop должно выполняться в целевом потоке");
        
        _targetServer.HardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _targetServer;

    public SoftStopCommand(ServerThread targetServer)
    {
        _targetServer = targetServer;
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _targetServer.WorkerThread)
            throw new InvalidOperationException("SoftStop должно выполняться в целевом потоке");
        
        _targetServer.SoftStop();
    }
}