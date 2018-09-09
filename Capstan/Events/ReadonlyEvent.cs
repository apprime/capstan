using System;
using System.Threading.Tasks;

namespace Capstan.Events
{
    public abstract class ReadingEvent
    {
        protected IEventResult Result;
        public static event EventHandler EventResolved;
        public delegate void EventHandler(IEventResult result);

        public async virtual Task<ReadingEvent> Process()
        {
            GatherData();
            Resolve();
            Broadcast();

            return this;            
        }

        /// <summary>
        /// Fetch or create the needed resources
        /// </summary>
        /// <returns></returns>
        protected abstract IEventResult GatherData();

        /// <summary>
        /// Create an eventResolution object for broadcasting
        /// </summary>
        /// <returns></returns>
        protected abstract IEventResult Resolve();

        /// <summary>
        /// Broadcast the change set to all relevant targets.
        /// </summary>
        protected virtual void Broadcast()
        {
            EventResolved?.Invoke(Result);
        }
    }
}
