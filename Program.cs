namespace PCSP_Assignment2;

class Program
{
    public static void Main()
    {
        ThreadPool threadPool = new(5);

        for (int i = 0; i < 10; i++)
        {
            threadPool.EnqueueTask(() =>
            {
                Console.WriteLine($"Task {i} is running on thread {Environment.CurrentManagedThreadId}");
                Thread.Sleep(1000);
            });
        }

        threadPool.Start();
    }
}