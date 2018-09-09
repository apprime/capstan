using System.Threading.Tasks;

namespace Capstan.Events
{
    /// <summary>
    /// An Event is a simple extension to ReadonlyEvent.
    /// It adds a method inside the Process which arranges for 
    /// mutated data to be set
    /// </summary>
    public abstract class MutatingEvent : ReadingEvent
    {
        public async override Task<ReadingEvent> Process()
        {
            GatherData();
            Resolve();
            Persist();
            Broadcast();

            return this;
        }

        /// <summary>
        /// Persist the change set in memory
        /// (i.e. make the changes)
        /// TODO: There is a major todo lurking here. 
        /// We need some way of locking resources and 
        /// rollbacking if they cant be committed.
        /// </summary>
        /// <returns></returns>
        protected abstract MutatingEvent Persist();
    }
}
