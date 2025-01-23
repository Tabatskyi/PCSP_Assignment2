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