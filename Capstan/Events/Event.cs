using Capstan;
using System.Threading.Tasks;

namespace Capstan.Events
{
    public abstract class CapstanEvent<IncomingType, ReturnedType> where IncomingType : Message
    {
        public abstract void Process();
        public abstract Task ProcessAsync();

        protected internal Broadcaster<IncomingType, ReturnedType> Broadcaster { set; protected get; }
        protected internal ErrorManager<ReturnedType> ErrorManager { set; protected get; }
    }
}
