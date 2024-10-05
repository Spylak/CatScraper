using System.Collections.Concurrent;

namespace CatScraper.Application.Helpers;

internal sealed class CatFetchLockHelper
{
    private static readonly Lazy<CatFetchLockHelper> LazyInstance = new(() => new CatFetchLockHelper());
    public static CatFetchLockHelper Instance => LazyInstance.Value;
    private readonly ConcurrentQueue<Func<Task>> _tasks = new ();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task AddTaskToQueue(Func<Task> action)
    {
        _tasks.Enqueue(action);
        await ExecuteTasks();
    }

    private async Task ExecuteTasks()
    {
        await _semaphore.WaitAsync();
        try
        {
            while (_tasks.TryDequeue(out var task))
            {
                await task();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private CatFetchLockHelper()
    {
    }
}