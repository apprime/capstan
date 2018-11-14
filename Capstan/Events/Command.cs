using System.Threading.Tasks;

namespace Capstan.Events
{
    /// <summary>
    /// An Event is a simple extension to ReadonlyEvent.
    /// It adds a method inside the Process which arranges for 
    /// mutated data to be set
    /// </summary>
    public abstract class Command : CapstanEvent
    {
        protected ICommandResult Result;

        public async Task<ICommandResult> Process()
        {
            await GatherData();
            await Resolve();
            await Persist();
            return Result;
        }

        /// <summary>
        /// Persist the change set in memory
        /// (i.e. make the changes)
        /// TODO: There is a major todo lurking here. 
        /// We need some way of locking resources and 
        /// rollbacking if they cant be committed.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<Command> Persist();
    }
}
