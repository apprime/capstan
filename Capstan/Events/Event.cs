using Capstan.Core;
using System.Threading.Tasks;

namespace Capstan.Events
{
    public abstract class CapstanEvent<TOutput>
    {
        public abstract void Process();
        public abstract Task ProcessAsync();

        protected internal Broadcaster<TOutput> Broadcaster { set; protected get; }
        protected internal ErrorManager<TOutput> ErrorManager { set; protected get; }
    }
}
