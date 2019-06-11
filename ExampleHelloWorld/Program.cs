using Capstan;
using System;

namespace ExampleHelloWorld
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //1. Create a builder for Capstan where the Input is of a specific type,
            //   and the output is a string.
            //Builder.New returns a new instance of the builder of the correct types.
            var capstan = Builder<HelloWorldInput, string>.New()

            //2. Add a route so that our client can send a message to this endpoint.
            //   It works similar to API routing [Route("/hello")]
            //   The input variable provided comes from our predefined input type. (ignore dependencies for now)
            .AddRoute("Hello", (input, dependencies) => new HelloWorldEvent(input.OurValue))

            //3. The broadcaster is an object that Capstan uses to send messages back to clients.
            //   you can inherit from a base class here to get needed functionality.
            //   The broadcaster is accessible from inside all of your events.
            .SetBroadcaster(dependencies => new Broadcaster<HelloWorldInput, string>())

            //4. The errormanager is an object that handles exceptions. Since we have no return values
            //   for any of our Events, we must return errors to the Sender with this instead.
            .SetErrorManager(dependencies => new HelloWorldErrorManager())

            //5. Build helps check that you did all of the above steps as a minimum and some other things before we start.
            .Build();


            //6. We create a client. This could be an object that keeps a websockets connection inside of it, or a service API endpoint
            //   Importantly, it has to persist because capstan may try to send a message to it at any time.
            var ourClient = new HelloWorldClient<string, string>(123);
            capstan.Subscribe(ourClient);

            //7. Start and Stop lets you pause and resume capstan.
            capstan.Start();

            //This part is about making the console app work
            //The important bit is where 'ourClient' sends a message to capstan.
            Console.WriteLine("Welcome to Hello [Word]");
            Console.WriteLine("Enter a Word for Capstan. Type 'exit' to stop.");
            string fromConsole = "";
            while (fromConsole.ToLower() != "exit")
            {
                fromConsole = Console.ReadLine();
                ourClient.Send("Hello", new HelloWorldInput(fromConsole));
            }
        }
    }
}
