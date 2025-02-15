using System;
using System.Collections.Generic;
using System.Threading;
using Zenject;

public class UnityMainThreadDispatcher: ITickable, IInitializable
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static SynchronizationContext _context;

    public static void Initialize()
    {
        _context = SynchronizationContext.Current;
    }

    public static void Enqueue(Action action)
    {
        if (_context == null)
        {
            throw new InvalidOperationException("UnityMainThreadDispatcher не инициализирован. Вызовите Initialize() в основном потоке.");
        }

        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    public void Tick()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                var action = _executionQueue.Dequeue();
                _context.Post(_ => action(), null);
            }
        }
    }

    void IInitializable.Initialize()
    {
        Initialize();
    }
}