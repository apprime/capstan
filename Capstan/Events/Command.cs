using System.Threading.Tasks;


/*
 The command is currently removed for the following reason:
 There is no way to pre-calculate which events need to persist data.
 Even a simple query might need to.

 Persisting is also not necessarily the last thing we do, or something
 we only do once.

Istead, let each event be responsible for managing its own resources.
Gather data when needed and persist when needed. This is more flexible and
also prevents us from locking ourselves into rigid inherited behaviour.

 */
//namespace Capstan.Events
//{
//    /// <summary>
//    /// An Event is a simple extension to ReadonlyEvent.
//    /// It adds a method inside the Process which arranges for 
//    /// mutated data to be set
//    /// </summary>
//    public abstract class Command : CapstanEvent
//    {
//        protected ICommandResult Result;

//        public async Task<ICommandResult> Process()
//        {
//            await Persist();
//            return Result;
//        }

//        /// <summary>
//        /// Persist the change set in memory
//        /// (i.e. make the changes)
//        /// TODO: There is a major todo lurking here. 
//        /// We need some way of locking resources and 
//        /// rollbacking if they cant be committed.
//        /// </summary>
//        /// <returns></returns>
//        protected abstract Task<Command> Persist();
//    }
//}
