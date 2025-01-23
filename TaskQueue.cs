namespace PCSP_Assignment2
{
    internal class TaskQueue(int capacity)
    {
        private readonly int capacity = capacity;
        private readonly Queue<Action> queue = new();

        public int Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (queue)
            {
                if (queue.Count >= capacity)
                { 
                    Console.WriteLine("Queue is full, task discarded");
                    return;
                }
                queue.Enqueue(action);
            }
        }

        public Action Dequeue()
        {
            lock (queue)
            {
                return queue.Dequeue();
            }
        }
    }
}