using System.Threading.Tasks;
using Capstan.Events;

namespace ExampleAllFeatures
{
    internal class ToAllEvent : CapstanEvent<StringMessage, string>
    {
        private StringMessage input;

        public ToAllEvent(StringMessage input)
        {
            this.input = input;
        }

        public override void Process()
        {
            this.Broadcaster
                .Filter(i => i.Id != Clients.Logger)
                .Broadcast("Good news everyone. We were supposed to make a delivery to the planet Tweenis 12 but it's been completely destroyed.");
        }

        public async override Task ProcessAsync()
        {
            this.Broadcaster.Broadcast("Good news everyone. We were supposed to make a delivery to the planet Tweenis 12 but it's been completely destroyed.");
        }
    }
}