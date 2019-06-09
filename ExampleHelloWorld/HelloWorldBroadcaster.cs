using Capstan.Core;
using System.Collections.Generic;

namespace ExampleHelloWorld
{
    public class HelloWorldBroadcaster : Broadcaster<HelloWorldInput,string>
    {
        private List<Client<HelloWorldInput, string>> _clients;

        public HelloWorldBroadcaster(List<Client<HelloWorldInput, string>> clients)
        {
            _clients = clients;
        }

        public override IEnumerable<Client<HelloWorldInput, string>> Clients => _clients;
    }
}
