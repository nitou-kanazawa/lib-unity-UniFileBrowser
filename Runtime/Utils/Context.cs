using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

internal sealed class Context<T> : IValueTaskSource<T>
{
    private ManualResetValueTaskSourceCore<T> _core;

    public ValueTask<T> Task => new(this, _core.Version);

    private Context()
    {
    }

    public T GetResult(short token)
    {
        var result = _core.GetResult(token);
        Context<T>.Return(this);
        return result;
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        return _core.GetStatus(token);
    }

    public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
    {
        _core.OnCompleted(continuation, state, token, flags);
    }

    public void SetResult(T result)
    {
        _core.SetResult(result);
    }

    public void SetException(Exception exception)
    {
        _core.SetException(exception);
    }

    #region Static

    private static readonly ConcurrentQueue<Context<T>> Pool = new();

    public static Context<T> Rent()
    {
        if (Pool.TryDequeue(out var context))
        {
            context._core.Reset();
            return context;
        }

        return new Context<T>();
    }

    public static void Return(Context<T> context)
    {
        Pool.Enqueue(context);
    }

    #endregion Static
}