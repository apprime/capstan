using System.Threading.Tasks;
using Capstan.Events;

namespace ExampleAllFeatures
{
    internal class TellOthersEvent : CapstanEvent<StringMessage, string>
    {
        private StringMessage input;

        public TellOthersEvent(StringMessage input)
        {
            this.input = input;
        }

        public override void Process()
        {
            this.Broadcaster.Filter(i => i.Id != input.SenderId).Broadcast($"Good news everyone, except {input.SenderName}, You'll be delivering a package to Chapek 9, a world where Humans are killed on sight.");
        }

        public async override Task ProcessAsync()
        {
            this.Broadcaster.Filter(i => i.Id != input.SenderId).Broadcast($"Good news everyone, except {input.SenderName}, You'll be delivering a package to Chapek 9, a world where Humans are killed on sight.");
        }
    }
}