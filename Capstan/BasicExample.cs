using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Capstan
{
    public class BasicExample
    {
        public void TestIntMain()
        {
            /*
             Scenario: 
             Our system can gather information from any number of sources.
             This information comes to our Engine class in the form of the input parameter.
             This input can be of any type(same as the type give to our Engine when we create it)
             In this example, it is an array of strings.
             The key is used to find the correct event to map this data to, the value is the information
             we want to send into the event when we create it. The client must know the name of the event
             when pushing things into the engine.

             SetBroadcaster expect you to provide a broadcaster instance that inherits from Broadcaster<TOutput>.
             It will allow your connected users to be notified when things happen.

             We then specify a number of routes by string name. There is currently no option to use other 
             types of keys than strings. For each key, we provide a factory method that will create the event for us.
             This factory method helps us put whatever we want inside of each constructor for each event, we can even 
             do special parsing of the data values we get as input

             We can either add a single route at a time, or send in an entire dictionary of routes. It comes down to preference.
             The type of the Event must implement from the CapstanEvent interface. Now we have set up the infrastructure required
             to turn (hopefully) all arrays of strings into Events, with data properly provided to them. The Engine can now use the 
             Process/ProcessAsync-methods as soon as possible and make the event happen.

             Once your events end up doing something that clients need to be notified of, get a hold of your broadcaster and broadcast 
             some information - a string in this example.
             */


            CapstanBuilder<string[], string>.New()
             .SetBroadcaster((clients) => new TestBroadcaster(clients))
             .RegisterActivist(new TestActivist())
             .ConfigRoute("Login", (evt) => new TestEvent(evt))
             .ConfigRoutes
             (
                new Dictionary<string, Func<string[], CapstanEvent>>
                {
                    { "Logout", (evt) => new TestEvent(evt)},
                    { "SomethingElse", (evt) => new TestEvent(evt)}
                }
             )
            //Obviously don't use the same names for multiple routes.
            //The engine wont throw exceptions but additional Events are not added.
            //Also, Synchronous events wont be allowed for final version.
            .ConfigRouteAsync("LoginAsync", (evt) => new TestEvent(evt))
            .ConfigRoutesAsync
             (
                new Dictionary<string, Func<string[], CapstanEvent>>
                {
                    { "LogoutAsync", (evt) => new TestEvent(evt)},
                    { "SomethingElseAsync", (evt) => new TestEvent(evt)}
                }
             )
             .Build();
        }
    }

    public class TestActivist : Activist
    {
        private int counter = 1;
        public async Task Activate()
        {
            await Task.Factory.StartNew(() => throw new NotImplementedException());
        }

        public bool Condition()
        {
            //Increase one every time we check, return true every 10 times.
            return ++counter % 10 == 0;
        }
    }

    public class TestEvent : CapstanEvent
    {
        public TestEvent(string[] something)
        {

        }

        public void Process()
        {
            throw new NotImplementedException();
        }

        public Task ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class TestUser : CapstanClient<string[], string>
    {
        public TestUser()
        {
        }

        public Subject<(string key, string[] value)> Messages => new Subject<(string key, string[] value)>();

        public void Receive(string output)
        {
            //Client takes string, parses it somehow if needed,
            //and does whatever it wants.
        }

        public void Send(string[] input)
        {
            //Somewhere someone tells CapstanClient to cc.Send(stuff);
            //This results in a message that is pushed into the grinder.
            Messages.OnNext(("Logout", input));
        }
    }

    public class TestBroadcaster : Broadcaster<string>
    {
        private readonly List<CapstanClient<string[], string>> innerDep;

        public TestBroadcaster(List<CapstanClient<string[], string>> users)
        {
            innerDep = users;
        }

        public override IEnumerable<CapstanReceiver<string>> Clients
        {
            get => innerDep;
        }
    }
}
