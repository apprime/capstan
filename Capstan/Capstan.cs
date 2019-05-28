using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace Capstan
{
    //Q: Should we allow synchronous events at all?
    //Q: TInput likely needs information about origin.
    public class Capstan<TInput, TOutput>
    {
        private Timer timer;
        private const int TickRate = 1000;

        /// <summary>
        /// Use the Instance method. It is designed to 
        /// work like a builder(TODO).
        /// </summary>
        private Capstan()
        {
            Events.Subscribe(async (i) => await Push(i));
        }

        public static Capstan<TInput, TOutput> Instance()
        {
            return new Capstan<TInput, TOutput>();
        }

        public Subject<(string, TInput)> Events { get; } = new Subject<(string, TInput)>();

        private async Task<IObserver<(string key, TInput value)>> Push((string key, TInput value) @event)
        {
            if (_routes.ContainsKey(@event.key))
            {
                //Create Event, call Sync
                _routes[@event.key](@event.value).Process();
            }

            if (_routesAsync.ContainsKey(@event.key))
            {
                //Create Event, call Async
                await _routesAsync[@event.key](@event.value).ProcessAsync();
            }

            //This should be logged and possibly returned to sender using TInput to figure that one out.
            //TODO: Figure out nice way to deal with errors in general. ErrorClient? Still needs origin of client.
            throw new ArgumentException(
                $"Incoming event with key {@event.key} does not exist as either an synchronous or asynchronous route." +
                $" Change the input key, or add a route to the engine during config, using either of the ConfigRoute methods.");
        }

        private List<CapstanClient<TInput, TOutput>> _clients = new List<CapstanClient<TInput, TOutput>>();
        public void Subscribe(CapstanClient<TInput, TOutput> client)
        {
            _clients.Add(client);
        }

        public void Unsubscribe(CapstanClient<TInput, TOutput> client)
        {
            _clients.Remove(client);
        }

        private void RegisterActivists()
        {
            timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, TickRate, TickRate);
        }

        public Capstan<TInput, TOutput> RegisterActivist(Core.Activist activist)
        {
            //TODO: Take factory method here. 
            // We want to be able to kill and regenerate activists.
            CapstanCycleEvent.RegisterActivist(activist);
            return this;
        }
        public Capstan<TInput, TOutput> RegisterActivists(params Core.Activist[] activists)
        {
            foreach (var activist in activists) { CapstanCycleEvent.RegisterActivist(activist); }
            return this;
        }

        private Dictionary<string, Func<TInput, CapstanEvent>> _routes = new Dictionary<string, Func<TInput, CapstanEvent>>();
        public Capstan<TInput, TOutput> ConfigRoute(string key, Func<TInput, CapstanEvent> eventFactory)
        {
            _routes.TryAdd(key, eventFactory);
            return this;
        }
        public Capstan<TInput, TOutput> ConfigRoutes(Dictionary<string, Func<TInput, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _routes.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        private Dictionary<string, Func<TInput, CapstanEvent>> _routesAsync = new Dictionary<string, Func<TInput, CapstanEvent>>();
        public Capstan<TInput, TOutput> ConfigRouteAsync(string key, Func<TInput, CapstanEvent> eventFactory)
        {
            _routesAsync.TryAdd(key, eventFactory);
            return this;
        }
        public Capstan<TInput, TOutput> ConfigRoutesAsync(Dictionary<string, Func<TInput, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _routesAsync.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        private Broadcaster<TOutput> _broadcaster = null;
        private Func<List<CapstanClient<TInput, TOutput>>, Broadcaster<TOutput>> _broadcasterFactory = null;
        public Broadcaster<TOutput> Broadcaster
        {
            get
            {
                if (_broadcaster == null)
                {
                    _broadcaster = _broadcasterFactory(_clients);
                }
                return _broadcaster;
            }
        }

        public Capstan<TInput, TOutput> SetBroadcaster(Func<List<CapstanClient<TInput, TOutput>>, Broadcaster<TOutput>> factory)
        {
            _broadcasterFactory = factory;
            return this;
        }

        public void Start()
        {
            if (_broadcasterFactory == null)
            {
                throw new Exception("Broadcaster has not been provided a factory method and cannot be created.");
            }

            RegisterActivists();
        }
    }

    public class A
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

            Capstan<string[], string>.Instance()
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
            //The engine wont accept a second route with the same sync/async type,
            //but you could technically add "abc" as a key to both sync and async routes.
            //This results in the synchronous event always running for the key "abc".
            .ConfigRouteAsync("LoginAsync", (evt) => new TestEvent(evt))
            .ConfigRoutesAsync
             (
                new Dictionary<string, Func<string[], CapstanEvent>>
                {
                    { "LogoutAsync", (evt) => new TestEvent(evt)},
                    { "SomethingElseAsync", (evt) => new TestEvent(evt)}
                }
             )
             .Start();
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
        private readonly Subject<(string, string[])> Events;
        public TestUser(Subject<(string, string[])> events)
        {
            Events = events;
        }

        public void Receive(string output)
        {
            //Client takes string, parses it somehow if needed,
            //and does whatever it wants.
        }

        public void Send(string[] input)
        {
            Events.OnNext(("Logout", new[] { "Goodbye", "Thanks", "Fish" }));
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
