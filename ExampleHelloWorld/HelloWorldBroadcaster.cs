using Capstan.Core;
using System.Collections.Generic;

namespace ExampleHelloWorld
{
    public class HelloWorldBroadcaster : Broadcaster<string>
    {
        private List<Receiver<string>> _clients;

        public HelloWorldBroadcaster(List<Receiver<string>> clients)
        {
            _clients = clients;
        }

        public override IEnumerable<Receiver<string>> Clients => _clients;
    }
}
