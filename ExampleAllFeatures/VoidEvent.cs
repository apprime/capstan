using System.Linq;
using System.Threading.Tasks;
using Capstan.Events;

namespace ExampleAllFeatures
{
    internal class VoidEvent : CapstanEvent<StringMessage, string>
    {
        private StringMessage input;

        public VoidEvent(StringMessage input)
        {
            this.input = input;
        }

        public override void Process()
        {
            Broadcaster.Filter(i => i.Id == Clients.Logger).Broadcast("The void consumes all.");
        }

        public async override Task ProcessAsync()
        {
            Broadcaster.Filter(i => i.Id == Clients.Logger).Broadcast("The void consumes all.");
        }
    }
}