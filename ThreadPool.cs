namespace PCSP_Assignment2;

internal class ThreadPool
{
    private int pendingTasks = 0;
    private bool allTasksCompleted = false;
    private bool isStopping = false;

    private readonly int workerThreads;
    private readonly TaskQueue queue;

    private readonly List<Task> tasks = [];
    private readonly Lock pendingTasksLock = new();
    private readonly object allTasksCompletedLock = new();

    public ThreadPool(int workerThreads = 0)
    {
        this.workerThreads = workerThreads == 0 ? Environment.ProcessorCount : workerThreads;
        queue = new TaskQueue();

        for (int i = 0; i < this.workerThreads; i++)
            tasks.Add(Task.Run(Worker));
    }

    private void Worker()
    {
        while (!isStopping)
        {
            Action? action = null;

            lock (queue)
            {
                while (queue.Count == 0 && !isStopping)
                     Monitor.Wait(queue);

                if (isStopping)
                    break;

                if (queue.Count > 0)
                {
                    action = queue.Dequeue();

                    lock (pendingTasksLock)
                        pendingTasks++;

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
                        setCompleted = true;
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

    public void Enqueue(Action action)
    {
        lock (queue)
        {
            queue.Enqueue(action);
            Monitor.Pulse(queue);
        }
    }

    public void Start()
    {
        lock (queue)
            Monitor.PulseAll(queue); 

        Console.WriteLine("ThreadPool started");
    }

    public void WaitAllTasks()
    {
        lock (allTasksCompletedLock)
            while (!allTasksCompleted)
                Monitor.Wait(allTasksCompletedLock);
    }

    public void Stop()
    {
        isStopping = true;
        lock (queue)
            Monitor.PulseAll(queue);

        Task.WaitAll([.. tasks]);
    }
}