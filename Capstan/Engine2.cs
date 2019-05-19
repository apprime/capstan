using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capstan
{
    //Q: Should we allow synchronous events at all?
    //Q: Current model is to use outside type for User, then create a broadcaster
    //   that handles this outside type. Should we instead have an internal broadcaster and a mapper?
    public class Engine2<TInput, TEventType, TUser>
    {
        private static event EventHandler<(string key, TEventType)> _incomingEvent;
        private Timer timer;
        private const int TickRate = 100;

        public Engine2()
        {
            _incomingEvent += OnIncomingEvent;
        }

        public void Push(TInput @event)
        {
            var mapped = _mapper(@event);
            OnIncomingEvent(null, mapped);
        }

        public async void OnIncomingEvent(object e, (string key, TEventType value) eventArgs)
        {
            IEventResult res;
            if (_routes.ContainsKey(eventArgs.key))
            {
                //Create Event, call Sync
                res = _routes[eventArgs.key](eventArgs.value).Process();
                Broadcaster.Set(res).Broadcast(eventArgs.value);
            }

            if (_routesAsync.ContainsKey(eventArgs.key))
            {
                //Create Event, call Async
                res = await _routesAsync[eventArgs.key](eventArgs.value).ProcessAsync();
                Broadcaster.Set(res).Broadcast(eventArgs.value);
            }

            //TODO: res should be put into the broadcaster. How does broadcaster know?

            throw new ArgumentException(
                $"Incoming event with key {eventArgs.key} does not exist as either an synchronous or asynchronous route." +
                $" Change the input key, or add a route to the engine during config, using either of the ConfigRoute methods.");
        }

        private void RegisterActivists()
        {
            timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, 1000, TickRate);
        }

        public Engine2<TInput, TEventType, TUser> RegisterActivist(IActivist activist)
        {
            CapstanCycleEvent.RegisterActivist(activist);
            return this;
        }
        public Engine2<TInput, TEventType, TUser> RegisterActivists(params IActivist[] activists)
        {
            foreach (var activist in activists) { CapstanCycleEvent.RegisterActivist(activist); }
            return this;
        }

        private Func<TInput, (string Key, TEventType Value)> _mapper;
        public Engine2<TInput, TEventType, TUser> SetMapper(Func<TInput, (string Key, TEventType Value)> mapper)
        {
            _mapper = mapper;
            return this;
        }

        private Dictionary<string, Func<TEventType, CapstanEvent>> _routes;
        public Engine2<TInput, TEventType, TUser> ConfigRoute(string key, Func<TEventType, CapstanEvent> eventFactory)
        {
            _routes.TryAdd(key, eventFactory);
            return this;
        }
        public Engine2<TInput, TEventType, TUser> ConfigRoutes(Dictionary<string, Func<TEventType, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _routes.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        private Dictionary<string, Func<TEventType, CapstanEvent>> _routesAsync;
        public Engine2<TInput, TEventType, TUser> ConfigRouteAsync(string key, Func<TEventType, CapstanEvent> eventFactory)
        {
            _routesAsync.TryAdd(key, eventFactory);
            return this;
        }
        public Engine2<TInput, TEventType, TUser> ConfigRoutesAsync(Dictionary<string, Func<TEventType, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _routesAsync.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        public Broadcaster<TEventType, TUser> Broadcaster { get; private set; }
        public Engine2<TInput, TEventType, TUser> SetBroadcaster(Broadcaster<TEventType, TUser> broadcaster)
        {
            Broadcaster = broadcaster;
            return this;
        }

        public void Start()
        {
            if(Broadcaster == null)
            {
                throw new Exception("Broadcaster has not been set.");
            }

            if(_mapper == null)
            {
                throw new Exception("Mapper has not been set.");
            }

            RegisterActivists();
        }
    }

    public class A
    {
        public void Tst()
        {
            var e = new Engine2<string[], string, object>();


            /*
             Scenario: 
             Our system can gather information from any number of sources.
             This information comes to our Engine class in the form of the input parameter.
             This input can be of any type(same as the type give to our Engine when we create it)
             In this example, it is an array of strings.

             The EngineMapper is a local method that takes this input and breaks it into a key and value.
             The key is used to find the correct event to map this data to, the value is the information
             we want to send into the event when we create it.

             Using the Engine.SetMapper() method to hand the method over to the Engine, so that it can be
             used internally.

             SetBroadcaster expect you to provide a broadcaster instance that inherits from Broadcaster<TPayload, TUser>.
             It will allow your connected users to be notified when things happen.

             We then specify a number of routes by string name. There is currently no option to use other 
             types of keys than strings. For each key, we provide a factory method that will create the event for us.
             This factory method helps us put whatever we want inside of each constructor for each event, we can even 
             do special parsing of the data values we get as input.

             We can either add a single route at a time, or send in an entire dictionary of routes. It comes down to preference.
             The type of the Event must be derived from the CapstanEvent class. Now we have set up the infrastructure required
             to turn (hopefully) all arrays of strings into Events, with data properly provided to them. The Engine can now use the 
             Process/ProcessAsync-methods as soon as possible and make the event happen.

             Every event will return an IEventResult, which only serves to tell the calling code whether the event was allowed to run
             and if it succesfully ran.
             */
            (string Key, string Value) EngineMapper(string[] input) => (Key: input[0], Value: input[1]);

            e.SetMapper(EngineMapper)
             .SetBroadcaster(new TestBroadcaster(null))
             .ConfigRoute("Login", (evt) => new TestEvent(evt))
             .ConfigRoutes
             (
                new Dictionary<string, Func<string, CapstanEvent>>
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
                new Dictionary<string, Func<string, CapstanEvent>>
                {
                    { "LogoutAsync", (evt) => new TestEvent(evt)},
                    { "SomethingElseAsync", (evt) => new TestEvent(evt)}
                }
             )
             .Start();
        }
    }

    public class Activist : IActivist
    {
        public void Activate()
        {
            throw new NotImplementedException();
        }

        public bool Condition()
        {
            throw new NotImplementedException();
        }
    }

    public class TestEvent : CapstanEvent
    {
        public TestEvent(object something)
        {

        }

        public override IEventResult Process()
        {
            throw new NotImplementedException();
        }

        public override Task<IEventResult> ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class TestBroadcaster : Broadcaster<string, object>
    {
        protected object _currentUser = null;
        private List<object> _allUsers = new List<object>();
        private readonly object _userService;
        private object _mappedObject;

        public TestBroadcaster(object userService)
        {
            _userService = userService;
        }

        protected override IEnumerable<object> FilterUsers(string payload)
        {
            //What is inside the Where is just a placeholder for real logic.
            return _allUsers.Where(i => payload.Contains((string)i));
        }

        internal override void Send(object user)
        {
            //Here we know what type User is, so we can use it to push a message
            //user.someMethod(_mappedObject);

            //Or, we have some internal service that can do it for us
            //_userService.sendMessage(user, _mappedObject);
        }

        protected override Broadcaster<string, object> Set(string payload)
        {
            //Apply some mapping logic and save the value
            _mappedObject = payload.Substring(3, 14);
            return this;
        }

        internal override Broadcaster<string, object> Set(IEventResult payload)
        {
            //Apply some mapping logic and save the value
            _mappedObject = payload.Resolution == EventResolutionType.Commit ? 3 : 14;
            return this;
        }

        protected override void Unset()
        {
            _mappedObject = null;
        }
    }
}
