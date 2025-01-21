namespace PCSP_Assignment2;

internal class ThreadPool
{
    private int pendingTasks = 0;
    private bool allTasksCompleted = false;

    private readonly int workerThreads;
    private readonly TaskQueue queue;

    private readonly List<Task> tasks = [];
    private readonly CancellationTokenSource cts = new();
    private readonly Lock pendingTasksLock = new();
    private readonly object allTasksCompletedLock = new();

    public ThreadPool(TaskQueue queue, int workerThreads = 0)
    {
        this.workerThreads = workerThreads == 0 ? Environment.ProcessorCount : workerThreads;
        this.queue = queue;

        for (int i = 0; i < this.workerThreads; i++)
        {
            tasks.Add(Task.Run(() => Worker(cts.Token), cts.Token));
        }
    }

    private void Worker(CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            Action? action = null;
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (queue.Count > 0)
                {
                    action = queue.Dequeue();
                    lock (pendingTasksLock)
                    {
                        pendingTasks++;
                    }
                    Console.WriteLine($"Task is running on thread {Environment.CurrentManagedThreadId}");
                }
            }

            action?.Invoke();

            if (action != null)
            {
                bool setCompleted = false;
                lock (pendingTasksLock)
                {
                    pendingTasks--;
                    if (pendingTasks == 0 && queue.Count == 0)
                    {
                        setCompleted = true;
                    }
                }
                if (setCompleted)
                {
                    lock (allTasksCompletedLock)
                    {
                        allTasksCompleted = true;
                        Monitor.PulseAll(allTasksCompletedLock);
                    }
                }
            }
        }
    }

    public void Start()
    {
        lock (queue)
        {
            Monitor.PulseAll(queue);
        }
        Console.WriteLine("ThreadPool started");
    }

    public void WaitAllTasks()
    {
        lock (allTasksCompletedLock)
        {
            while (!allTasksCompleted)
            {
                Monitor.Wait(allTasksCompletedLock);
            }
        }
    }

    public void Stop()
    {
        cts.Cancel();
        lock (queue)
        {
            Monitor.PulseAll(queue);
        }

        Task.WaitAll([.. tasks]);
    }
}