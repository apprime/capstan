using System.Threading.Tasks;

namespace Capstan.Events
{
    public abstract class CapstanEvent
    {
        internal CapstanEvent()
        {
            //Prevent plebs from creating this outside of assembly.
        }

        public abstract Task<IEventResult> Process();
    }
}
