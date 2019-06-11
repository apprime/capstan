using System.Threading.Tasks;
using Capstan.Events;

namespace ExampleAllFeatures
{
    internal class BounceEvent : CapstanEvent<StringMessage, string>
    {
        private StringMessage input;

        public BounceEvent(StringMessage input)
        {
            this.input = input;
        }

        public override void Process()
        {
            this.Broadcaster.ToSender(input, $"Dear {input.SenderName}, We really loved your message but please don't contact us again.");
        }

        public async override Task ProcessAsync()
        {
            this.Broadcaster.ToSender(input, $"Dear {input.SenderName}, We really loved your message but please don't contact us again.");

        }
    }
}