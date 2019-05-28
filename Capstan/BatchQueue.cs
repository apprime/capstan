using System.Collections.Generic;

namespace Capstan
{
    //TODO: This exists because we probably want to store all incoming events in a queue.
    //      Then process a number at the time, so that we can find duplicates and such with much
    //      impressive logic.
    public class BatchQueue<T> : Queue<T>
    {
        public IEnumerable<T> DequeueChunk(int chunkSize)
        {
            for (var i = 0; i < chunkSize && this.Count > 0; i++)
            {
                yield return Dequeue();
            }
        }
    }
}
