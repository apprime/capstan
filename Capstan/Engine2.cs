using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capstan
{
    public class Engine2<TInput, TEventType>
    {
        private const int TickRate = 100;
        private Timer timer;

        //We need to add a parser and a router.

        public async Task<IEventResult> Push(TInput @event)
        {
            var mapped = _mapper(@event);

            if(_routes.ContainsKey(mapped.Key))
            {
                //Create Event, call Sync
                return _routes[mapped.Key](mapped.Value).Process();
            }

            if(_routesAsync.ContainsKey(mapped.Key))
            {
                return await _routesAsync[mapped.Key](mapped.Value).ProcessAsync();
            }

            throw new ArgumentException(
                $"Incoming event with key {mapped.Key} does not exist as either an synchronous or asynchronous route. " +
                $"Change the input key, or add a route to the engine during config, using either of the ConfigRoute methods.");
        }

        private Dictionary<Type, Action<object>> _reactions;
        public Engine2<TInput, TEventType> RegisterReactionary<T>(Func<IReactionary<T>> setHandler)
        {
            _reactions.Add(typeof(T), (evt) => setHandler().Subscribe((IRaiseEvent<T>)evt));
            return this;
        }

        private void RegisterActivists()
        {
            timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, 1000, TickRate);
        }

        public Engine2<TInput, TEventType> RegisterActivist(IActivist activist)
        {
            CapstanCycleEvent.RegisterActivist(activist);
            return this;
        }

        public Engine2<TInput, TEventType> RegisterActivists(params IActivist[] activists)
        {
            foreach (var activist in activists) { CapstanCycleEvent.RegisterActivist(activist); }
            return this;
        }

        private Func<TInput, (string Key, TEventType Value)> _mapper;
        public Engine2<TInput, TEventType> SetMapper(Func<TInput, (string Key, TEventType Value)> mapper)
        {
            _mapper = mapper;
            return this;
        }

        private Dictionary<string, Func<TEventType, CapstanEvent>> _routes;
        public Engine2<TInput, TEventType> ConfigRoute(string key, Func<TEventType, CapstanEvent> eventFactory)
        {
            _routes.TryAdd(key, eventFactory);
            return this;
        }

        public Engine2<TInput, TEventType> ConfigRoutes(Dictionary<string, Func<TEventType, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _routes.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        private Dictionary<string, Func<TEventType, CapstanEvent>> _routesAsync;
        public Engine2<TInput, TEventType> ConfigRouteAsync(string key, Func<TEventType, CapstanEvent> eventFactory)
        {
            _routesAsync.TryAdd(key, eventFactory);
            return this;
        }

        public Engine2<TInput, TEventType> ConfigRoutesAsync(Dictionary<string, Func<TEventType, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _routesAsync.TryAdd(route.Key, route.Value);
            }

            return this;
        }
    }

    public interface IMapper<TInput, TEventData>
    {
        KeyValuePair<string, TEventData> Map(TInput input);
    }

    public class A : IMapper<string, int>
    {
        public KeyValuePair<string, int> Map(string input)
        {
            return new KeyValuePair<string, int>("Hello", 4);
        }

        public void Tst()
        {
            var e = new Engine2<string[], string>();


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
             .ConfigRoute("Login", (evt) => new ReactToLordZorkelbort(evt))
             .ConfigRoutes
             (
                new Dictionary<string, Func<string, CapstanEvent>>
                {
                    { "Logout", (evt) => new ReactToLordZorkelbort(evt)},
                    { "SomethingElse", (evt) => new ReactToLordZorkelbort(evt)}
                }
             )
            .ConfigRouteAsync("LoginAsync", (evt) => new ReactToLordZorkelbort(evt))
            .ConfigRoutesAsync
             (
                new Dictionary<string, Func<string, CapstanEvent>>
                {
                    { "LogoutAsync", (evt) => new ReactToLordZorkelbort(evt)},
                    { "SomethingElseAsync", (evt) => new ReactToLordZorkelbort(evt)}
                }
             );
        }
    }

    /// <summary>
    /// An event reactionary is a class that reacts
    /// to events raised by the system. They do not
    /// need to do anything specific, all we really 
    /// know is that they are triggered by events.
    /// </summary>
    public class ReactToLordZorkelbort : CapstanEvent, IReactionary<LordZorkelbortIsBack>
    {
        public ReactToLordZorkelbort(string data)
        {

        }

        public void Subscribe(IRaiseEvent<LordZorkelbortIsBack> @event)
        {
            @event.OnEvent += Handler;
        }

        public void Handler(object e, LordZorkelbortIsBack args)
        {
            // He is too strong
            // There is nothing that can handle the lord!
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

    public class LordZorkelbortHappens : IRaiseEvent<LordZorkelbortIsBack>
    {
        public event EventHandler<LordZorkelbortIsBack> OnEvent;
        public void Main()
        {
            OnEvent?.Invoke(null, new LordZorkelbortIsBack());
        }
    }

    public class LordZorkelbortIsBack : EventArgs
    {
        public int Important { get; set; } = 4;
    }
}
