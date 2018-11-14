using System.Collections.Generic;

namespace Capstan
{
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
