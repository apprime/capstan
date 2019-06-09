using System.Threading.Tasks;

namespace ExampleHelloWorld
{
    public class HelloWorldEvent : Capstan.Events.CapstanEvent<string>
    {
        private readonly string _value;

        public HelloWorldEvent(string ourValue)
        {
            _value = ourValue;
        }

        public override void Process()
        {
            //If we want to send this message to all clients
            //Broadcaster.Broadcast($"Hello {_value}!");

            //antoher option could be to filter the clients
            Broadcaster.Filter(i => i.Id == 123).Broadcast($"Hello {_value}!");
        }

        public override async Task ProcessAsync()
        {
           Broadcaster.Broadcast($"Hello {_value}!");
        }
    }
}
