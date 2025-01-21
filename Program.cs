namespace PCSP_Assignment2;

class Program
{
    private static readonly Random random = new();

    public static void Main()
    {
        const int totalTasks = 20;
        const int workerThreads = 6;
        const int minTime = 5000;
        const int maxTime = 10000;
        TaskQueue taskQueue = new();

        for (int i = 1; i <= totalTasks; i++)
        {
            int taskNumber = i;
            taskQueue.Enqueue(() =>
            {
                int time = random.Next(minTime, maxTime);
                Thread.Sleep(time);
                Console.WriteLine($"Task {taskNumber} is done with time: {time}ms");
            });
        }

        ThreadPool threadPool = new(taskQueue, workerThreads);
        threadPool.Start();

        threadPool.WaitAllTasks();

        threadPool.Stop();
    }
}