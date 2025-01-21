namespace PCSP_Assignment2
{
    internal class TaskQueue
    {
        private readonly Queue<Action> queue = new();
        public int Count => queue.Count;

        public void Enqueue(Action action)
        {
            lock (queue)
            {
                queue.Enqueue(action);
                Monitor.Pulse(queue);
            }
        }

        public Action Dequeue()
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }
                return queue.Dequeue();
            }
        }

        public void PulseAll()
        {
            lock (queue)
            {
                Monitor.PulseAll(queue);
            }
        }

        public void Clear()
        {
            lock (queue)
            {
                queue.Clear();
            }
        }

        public void Pulse()
        {
            lock (queue)
            {
                Monitor.Pulse(queue);
            }
        }
    }
}
