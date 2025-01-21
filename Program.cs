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
        CountdownEvent countdown = new(totalTasks);

        for (int i = 1; i <= totalTasks; i++)
        {
            int taskNumber = i;
            taskQueue.Enqueue(() =>
            {
                int time = random.Next(minTime, maxTime);
                Thread.Sleep(time);
                Console.WriteLine($"Task {taskNumber} is done with time: {time}ms");
                countdown.Signal();
            });
        }

        ThreadPool threadPool = new(taskQueue, workerThreads);
        threadPool.Start();

        countdown.Wait();

        //threadPool.Stop();
        //threadPool.WaitAll();
    }
}
