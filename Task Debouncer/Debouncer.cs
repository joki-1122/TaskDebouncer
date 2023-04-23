using System.Collections.Concurrent;

namespace Task_Debouncer;

public sealed class Debouncer
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _pendingTasks;

    public Debouncer()
    {
        _pendingTasks = new ConcurrentDictionary<string, TaskCompletionSource<object>>();
    }

    public async Task<T> DebounceAsync<T>(string requestName, Func<Task<T>> task)
    {
        if (_pendingTasks.TryGetValue(requestName, out var taskCompletionSource))
            return (T)await taskCompletionSource!.Task;

        taskCompletionSource = new TaskCompletionSource<object>();
        
        if (!_pendingTasks.TryAdd(requestName, taskCompletionSource)) 
            return (T)await _pendingTasks[requestName].Task;
        
        try
        {
            var result = await task();
            taskCompletionSource.SetResult(result);
        }
        finally
        {
            _pendingTasks.TryRemove(requestName, out _);
        }

        return (T)await taskCompletionSource.Task;
    }
}