using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace Capstan
{
    public class Capstan<TInput, TOutput> where TInput : CapstanMessage
    {
        private const int TickRate = 1000;
        private bool _started;
        private ErrorManager<TInput, TOutput> _errorManager;
        private Broadcaster<TOutput> _broadcaster;
        private List<(CapstanClient<TInput, TOutput> client, IDisposable subscription)> _clients;
        private Timer _timer;
        private IUnityContainer _dependencyContainer;

        /// <summary>
        /// This is private.
        /// Use the CapstanBuilder class. 
        /// It will help you set everything up properly.
        /// </summary>
        internal Capstan()
        {
            _started = false;
            _clients = new List<(CapstanClient<TInput, TOutput> client, IDisposable subscription)>();
            _broadcaster = null;
            _errorManager = null;
            _timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, TickRate, TickRate);
            _dependencyContainer = new UnityContainer();
            RoutesAsync = new Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent>>();
            Routes = new Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent>>();
        }

        public IUnityContainer Dependencies { get; internal set; }
        public Broadcaster<TOutput> Broadcaster
        {
            get
            {
                if (_broadcaster == null)
                {
                    _broadcaster = BroadcasterFactory(_clients.Select(i => i.client).Cast<CapstanReceiver<TOutput>>().ToList());
                }
                return _broadcaster;
            }
        }
        public ErrorManager<TInput, TOutput> ErrorManager
        {
            get
            {
                if (_errorManager == null)
                {
                    _errorManager = ErrorManagerFactory(_clients.Select(i => i.client).ToList());
                }
                return _errorManager;
            }
        }

        public void Subscribe(CapstanClient<TInput, TOutput> client)
        {
            var subscription = client.Messages.Subscribe(async (i) => await Push(i));
            _clients.Add((client, subscription));
        }
        public void Unsubscribe(CapstanClient<TInput, TOutput> client)
        {
            var currentClient = _clients
                .Where(i => i.client == client)
                .Single();

            currentClient.subscription.Dispose();
            _clients.Remove(currentClient);
        }
        public void Start()
        {
            _started = true;
        }
        public void Stop()
        {
            _started = false;
        }

        internal void RegisterActivists(params Activist[] activists)
        {
            foreach (var activist in activists) { CapstanCycleEvent.RegisterActivist(activist); }
        }
        internal void SetErrorManager(ErrorManager<TInput, TOutput> errorManager)
        {
            _errorManager = errorManager;
        }
        internal Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent>> Routes { get; }
        internal Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent>> RoutesAsync { get; }
        internal Func<List<CapstanReceiver<TOutput>>, Broadcaster<TOutput>> BroadcasterFactory { get; set; }
        internal Func<List<CapstanClient<TInput, TOutput>>, ErrorManager<TInput, TOutput>> ErrorManagerFactory { get; set; }

        private async Task Push((string key, TInput value) @event)
        {
            if (!_started) { return; }

            try
            {
                bool found = false;
                if (Routes.ContainsKey(@event.key))
                {
                    //Create Event, call Sync
                    Routes[@event.key](@event.value, Dependencies).Process();
                    found = true;
                }

                if (RoutesAsync.ContainsKey(@event.key))
                {
                    //Create Event, call Async
                    await RoutesAsync[@event.key](@event.value, Dependencies).ProcessAsync();
                    found = true;
                }

                if (!found)
                {
                    ErrorManager.ReturnToSender(@event, new ArgumentException(
                $"Incoming event with key {@event.key} does not exist as either an synchronous or asynchronous route." +
                $" Change the input key, or add a route to the engine during config, using either of the ConfigRoute methods."));
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ReturnToSender(@event, ex);
            }
        }

    }
}
