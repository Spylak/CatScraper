namespace CatScraper.Application.Helpers;

public class CatFetchLockHelper
{
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public async Task RunWithLockAsync(Func<Task> action)
    {
        await _lock.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            _lock.Release();
        }
    }
}