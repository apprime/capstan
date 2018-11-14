using System.Threading.Tasks;

namespace Capstan.Events
{
    public abstract class CapstanEvent
    {
        internal CapstanEvent()
        {
            //Prevent plebs from creating this outside of assembly.
        }

        //public static event EventHandler EventResolved;
        //public delegate void EventHandler(IEventResult result);
        public abstract Task<ICommandResult> GatherData();
        //public abstract Task<ICommandResult> Process();
        public abstract Task<ICommandResult> Resolve();
    }
}
