namespace PCSP_Assignment2
{
    internal class TaskQueue(int capacity)
    {
        private readonly Queue<Action> queue = new(capacity);

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
                if (queue.Count >= queue.Capacity)
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