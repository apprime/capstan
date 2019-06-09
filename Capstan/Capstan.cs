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
    public class Capstan<TInput, TOutput> where TInput : Message
    {
        private const int TickRate = 1000;
        private bool _started;
        private ErrorManager<TOutput> _errorManager;
        private Broadcaster<TOutput> _broadcaster;
        private List<(Client<TInput, TOutput> client, IDisposable subscription)> _clients;
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
            _clients = new List<(Client<TInput, TOutput> client, IDisposable subscription)>();
            _broadcaster = null;
            _errorManager = null;
            _timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, TickRate, TickRate);
            _dependencyContainer = new UnityContainer();
            RoutesAsync = new Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent<TOutput>>>();
            Routes = new Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent<TOutput>>>();
        }

        public IUnityContainer Dependencies { get; internal set; }
        public Broadcaster<TOutput> Broadcaster
        {
            get
            {
                if (_broadcaster == null)
                {
                    _broadcaster = BroadcasterFactory(ConvertClientsToReceivers(), Dependencies);
                }
                return _broadcaster;
            }
        }

        public ErrorManager<TOutput> ErrorManager
        {
            get
            {
                if (_errorManager == null)
                {
                    _errorManager = ErrorManagerFactory(ConvertClientsToDictionaryOfReceivers(), Dependencies);
                }
                return _errorManager;
            }
        }

        public void Subscribe(Client<TInput, TOutput> client)
        {
            var subscription = client.Messages.Subscribe(async (i) => 
            {
                await Push(i);
            });
            _clients.Add((client, subscription));
        }
        public void Unsubscribe(Client<TInput, TOutput> client)
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

        internal void SetErrorManager(ErrorManager<TOutput> errorManager)
        {
            _errorManager = errorManager;
        }
        internal Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent<TOutput>>> Routes { get; }
        internal Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent<TOutput>>> RoutesAsync { get; }
        internal Func<List<Receiver<TOutput>>, IUnityContainer, Broadcaster<TOutput>> BroadcasterFactory { get; set; }
        internal Func<Dictionary<int, Receiver<TOutput>>, IUnityContainer, ErrorManager<TOutput>> ErrorManagerFactory { get; set; }

        private async Task Push((string key, TInput value) @event)
        {
            if (!_started) { return; }

            try
            {
                bool found = false;
                if (Routes.ContainsKey(@event.key))
                {
                    //Create Event, call Sync
                    var evt = Routes[@event.key](@event.value, Dependencies);
                    evt.Broadcaster = Broadcaster;
                    evt.ErrorManager = ErrorManager;
                    evt.Process();
                    found = true;
                }

                if (RoutesAsync.ContainsKey(@event.key))
                {
                    //Create Event, call Async
                    var evt = RoutesAsync[@event.key](@event.value, Dependencies);
                    evt.Broadcaster = Broadcaster;
                    evt.ErrorManager = ErrorManager;
                    await evt.ProcessAsync();
                    found = true;
                }

                if (!found)
                {
                    throw ErrorManager.ArgumentException(@event.key);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ReturnToSender(@event.value.SenderId, ex);
            }
        }

        private List<Receiver<TOutput>> ConvertClientsToReceivers()
        {
            return _clients
                .Select(i => i.client)
                .Cast<Receiver<TOutput>>()
                .ToList();
        }

        private Dictionary<int, Receiver<TOutput>> ConvertClientsToDictionaryOfReceivers()
        {
            return _clients
                .Select(i => i.client)
                .ToDictionary(i => i.Id, i => (Receiver<TOutput>)i);
        }
    }
}
