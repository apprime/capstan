using Capstan;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace ExampleAllFeatures
{
    partial class Program
    {
        private static bool running = false;
        private static Capstan<StringMessage, string> capstan = null;
        private static List<GenericClient> clients = new List<GenericClient>();
        private static int clientCount = 0;

        static void Main(string[] args)
        {
            Setup();
            //1. Register Clients with a name and id
            //2. Register Second Client for logging only.
            //4. Register Event with no dependencies
            //5. Register Event with single dependency
            //6. Register Event with complicated dependency

            //9. Register Activist that pokes random person on a timer

            //1. Send Message that throws error
            //2. Send Message that returns to sender
            //3. Send Message that returns to all
            //4. Send Message with no returns
            //5. Send Message with several returns to sender

            //1. Receive a Message from Activist
            //2. Unsubscribe to get away from Activist
            //3. Stop Capstan, resubscribe and see that Activist is no longer working.

            while(running)
            {
                ReadInput();
            }
            
        }

        private static void Setup()
        {
            capstan =
                Builder<StringMessage, string>.New()
                .AddRoute("Error", (input, dependencies) => new ErrorEvent(input))
                .AddRoute("ReturnToMe", (input, dependencies) => new BounceEvent(input))
                .AddRoute("TellAll", (input, dependencies) => new ToAllEvent(input))
                .AddRoute("TellOthers", (input, dependencies) => new TellOthersEvent(input))
                .AddRoute("Void", (input, dependencies) => new VoidEvent(input))
                .AddRoute("TellMeMuch", (input, dependencies) => new ChattyEvent(input))
                .RegisterDependencies(container =>
                {
                    //Remember: using Unity;
                    container.RegisterType<IEnterpriseBusinessDependency, EnterpriseBusinessDependency>(new TransientLifetimeManager());
                })
                .RegisterActivist((dependencies) => new PeoplePoker<StringMessage, string>())
                .SetBroadcaster(dependencies => new Broadcaster<StringMessage, string>())
                .SetErrorManager(dependencies => new FeaturedErrorManager())
                .Build();

            capstan.Subscribe(new LoggerClient(1337, "Logger"));
        }

        private static void ReadInput()
        {
            var input = Console.ReadLine();

            if(input == "exit") { running = false; }
            if(input == "start") { capstan.Start(); }
            if(input == "stop") { capstan.Stop(); }

            //Parse input for 'add "name"'
            //string clientName;
            //AddClient(clientName);
            //Parse input for 'subscribe "name"'
            //Parse input for 'unsubscribe "name"'
            //Parse input for 'list clients'
            //Parse input for 'send "key" "message" "name"'

        }

        private static void AddClient(string clientName)
        {
            clientCount++;
            clients.Add(new GenericClient(clientCount, clientName));
        }

        private static void ListClients()
        {
            Console.WriteLine("Current Existing users are:");
            Console.WriteLine(string.Join(",", clients.Select(i => i.Name)));
        }

        private static void SubscribeClient(string clientName)
        {
            var c = clients.Single(i => i.Name == clientName);
            if(c != null)
            {
                capstan.Subscribe(c);
            }
        }

        private static void UnsubscribeClient(string clientName)
        {
            var c = clients.Single(i => i.Name == clientName);
            if (c != null)
            {
                capstan.Unsubscribe(c);
            }
        }

        private static void SendMessage(string key, string body, GenericClient client)
        {
            Console.WriteLine($"Sending {key}: {body}");
            client.Send(key, new StringMessage(body));
        }
    }
}
