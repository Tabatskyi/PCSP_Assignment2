namespace PCSP_Assignment2;

internal class ThreadPool
{
    private readonly int workerThreads;
    private readonly TaskQueue queue = new();
    private readonly List<Task> tasks = [];
    private readonly CancellationTokenSource cts = new();

    public ThreadPool(int workerThreads = 0)
    {
        this.workerThreads = workerThreads == 0 ? Environment.ProcessorCount : workerThreads;

        for (int i = 0; i < this.workerThreads; i++)
        {
            tasks.Add(Task.Run(() => Worker(cts.Token), cts.Token));
        }
    }

    private void Worker(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Action? action = null;
            lock (queue)
            {
                while (queue.Count == 0 && !token.IsCancellationRequested)
                {
                    Monitor.Wait(queue);
                }
                if (queue.Count > 0)
                {
                    action = queue.Dequeue();
                    Console.WriteLine($"Task is running on thread {Environment.CurrentManagedThreadId}");
                }
            }
            action?.Invoke();
        }
    }

    public void EnqueueTask(Action action)
    {
        queue.Enqueue(action);
        Console.WriteLine($"Task enqueued on thread {Environment.CurrentManagedThreadId}");
    }

    public void Start()
    {
        queue.PulseAll();
    }

    public void WaitAll()
    {
        foreach (var task in tasks)
        {
            task.Wait();
        }
    }

    public void Stop()
    {
        cts.Cancel();
        queue.PulseAll();
        Task.WaitAll([.. tasks]);
    }
}

