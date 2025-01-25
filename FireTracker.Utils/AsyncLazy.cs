namespace FireTracker.Utils;

public class AsyncLazy(Func<Task> factory)
{
    private readonly Lazy<Task> _instance = new(() => Task.Run(factory));

    public Task Complete => _instance.Value;
}