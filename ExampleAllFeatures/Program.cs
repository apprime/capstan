using Capstan;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace ExampleAllFeatures
{
    internal partial class Program
    {
        private static bool running = true;
        private static Capstan<StringMessage, string> capstan = null;
        private static List<GenericClient> clients = new List<GenericClient>();
        private static int clientCount = 0;

        private static void Main(string[] args)
        {
            Setup();
            //Register Event with no dependencies
            //Register Event with single dependency
            //Register Event with complicated dependency

            Console.WriteLine("Capstan is ready, but not started. For help, type 'list commands'");
            while (running)
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
                .AddRoute("TellMeMuch", (input, dependencies) => new ChattyEventFactory().Create(input, dependencies)) //Create an instance of the factory, or use static method Create.
                .RegisterDependencies(container =>
                {
                    //Remember: using Unity;
                    container.RegisterType<IEnterpriseBusinessDependency, EnterpriseBusinessDependency>(new TransientLifetimeManager());
                })
                .RegisterActivist((dependencies) => new PeoplePoker<StringMessage, string>()) //Register Activist that pokes random person on a timer
                .SetBroadcaster(dependencies => new Broadcaster<StringMessage, string>())
                .SetErrorManager(dependencies => new FeaturedErrorManager())
                .Build();

            capstan.Subscribe(new LoggerClient(1337, "Logger"));
        }

        private static void ReadInput()
        {
            var input = Console.ReadLine();

            if (input == "exit") { running = false; }
            if (input == "start") { capstan.Start(); }
            if (input == "stop") { capstan.Stop(); }

            if (input.StartsWith("list"))
            {
                var parts = input.Split(' ');
                if (parts.Length == 2)
                {
                    if (parts[1] == "clients") { ListClients(); }
                    else if (parts[1] == "messages") { ListMessages(); }
                    else if (parts[1] == "commands") { ListCommands(); }
                    else
                    {
                        Console.WriteLine("Use either 'list clients', 'list commands' or 'list messages'");
                    }
                }
                else
                {
                    Console.WriteLine("Use either 'list clients', 'list commands' or 'list messages'");
                }

            }

            if (input.StartsWith("add"))
            {
                var parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Add Clients using only the keyword 'add' and a name, separated by space");
                }
                else
                {
                    AddClient(parts[1]);
                    Console.WriteLine($"Added Client {parts[1]}");
                }
            }

            if (input.StartsWith("subscribe"))
            {
                var parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Subscribe Clients using only the keyword 'subscribe' and a name, separated by space");
                }
                else
                {
                    if (clients.Any(i => i.Name == parts[1]))
                    {
                        SubscribeClient(parts[1]);
                        Console.WriteLine($"Subscribed Client {parts[1]}");
                    }
                    else
                    {
                        Console.WriteLine($"Cannot find a client named {parts[1]}, did you add it?");
                    }
                }
            }

            if (input.StartsWith("unsubscribe"))
            {
                var parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Unsubscribe Clients using only the keyword 'unsubscribe' and a name, separated by space");
                }
                else
                {
                    if (clients.Any(i => i.Name == parts[1]))
                    {
                        UnsubscribeClient(parts[1]);
                        Console.WriteLine($"Unsubscribe Client {parts[1]}");
                    }
                    else
                    {
                        Console.WriteLine($"Cannot find a client named {parts[1]}, did you add it?");
                    }
                }
            }

            if (input.StartsWith("send"))
            {
                var parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Send messages with this syntax: 'send [key]' ie 'send tellall'");
                }
                else
                {
                    var key = parts[1];
                    Console.WriteLine("Which client is this?");
                    var clientStr = Console.ReadLine();
                    var client = clients.SingleOrDefault(i => i.Name == clientStr);
                    if (client != null)
                    {
                        Console.WriteLine("What is your message?");
                        var body = Console.ReadLine();

                        SendMessage(key, body, client);

                        Console.WriteLine($"Sending a {key} message from {clientStr} with the contents '{body}'.");
                    }
                    else
                    {
                        Console.WriteLine($"Cannot find a client named {parts[1]}, did you add it?");
                    }
                }
            }
        }

        private static void AddClient(string clientName)
        {
            clientCount++;
            clients.Add(new GenericClient(clientCount, clientName));
        }

        private static void ListCommands()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("Add [name] - Add a client to program");
            Console.WriteLine("Remove [name] - Remove a client from program");
            Console.WriteLine("Subscribe [name] - Push a client into capstan");
            Console.WriteLine("Unsubscribe [name] - Remove a client from capstan");
            Console.WriteLine("Send [message name] - Start the process of sending a message");
            Console.WriteLine("Start - Tell capstan to begin listening for commands");
            Console.WriteLine("Stop - Tell capstan to stop listening for commands");
            Console.WriteLine("Exit - terminate the program");
            Console.WriteLine("List Commands - List all commands");
            Console.WriteLine("List Clients - List all Clients");
            Console.WriteLine("List Messages - List all message types");
        }

        private static void ListClients()
        {
            Console.WriteLine("Current Existing users are:");
            Console.WriteLine(string.Join(",", clients.Select(i => i.Name)));
        }

        private static void ListMessages()
        {
            Console.WriteLine("Available Messages:");
            Console.WriteLine("[Error] - Message that throws error");
            Console.WriteLine("[ReturnToMe] - Message that returns to sender");
            Console.WriteLine("[TellAll] - Message that returns to all");
            Console.WriteLine("[TellOthers] - Message that returns to all except me");
            Console.WriteLine("[Void] - Message with no returns");
            Console.WriteLine("[TellMeMuch] - Message with multiple returns");
        }

        private static void SubscribeClient(string clientName)
        {
            var c = clients.Single(i => i.Name == clientName);
            if (c != null)
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
            client.Send(key, new StringMessage(body));
        }
    }
}
