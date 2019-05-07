using System.Threading.Tasks;

namespace Capstan.Events
{
    //TODO: This can currently be an interface. Maybe even two separate for sync and async?
    public abstract class CapstanEvent
    {
        internal CapstanEvent()
        {
            //Prevent plebs from creating this outside of assembly.
        }

        public abstract IEventResult Process();
        public abstract Task<IEventResult> ProcessAsync();
    }
}
