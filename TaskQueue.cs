namespace PCSP_Assignment2
{
    internal class TaskQueue
    {
        private readonly Queue<Action> queue = new();

        public int Count
        {
            get
            {
                lock (queue)
                    return queue.Count;
            }
        }

        public void Enqueue(Action action) => queue.Enqueue(action);
        public Action Dequeue() => queue.Dequeue();
    }
}