using System.Threading.Tasks;
using Capstan.Events;

namespace ExampleAllFeatures
{
    internal class ErrorEvent : CapstanEvent<StringMessage, string>
    {
        private StringMessage input;

        public ErrorEvent(StringMessage input)
        {
            this.input = input;
        }

        public override void Process()
        {
            throw new System.Exception($"Dear {input.SenderName}. This ErrorEvent threw and error and successfully failed!");
        }

        public async override Task ProcessAsync()
        {
            throw new System.Exception("This ErrorEvent threw and error and successfully failed!");
        }
    }
}