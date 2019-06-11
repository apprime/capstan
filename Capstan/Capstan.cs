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
    public class Capstan<IncomingType, ReturnedType> where IncomingType : Message
    {
        private const int TickRate = 1000;
        private bool _started;
        private ErrorManager<ReturnedType> _errorManager;
        private Broadcaster<IncomingType, ReturnedType> _broadcaster;
        private List<(Client<IncomingType, ReturnedType> client, IDisposable subscription)> _clients;
        private readonly Timer _timer;
        private readonly IUnityContainer _dependencyContainer;

        /// <summary>
        /// This is private.
        /// Use the CapstanBuilder class. 
        /// It will help you set everything up properly.
        /// </summary>
        internal Capstan()
        {
            _started = false;
            _clients = new List<(Client<IncomingType, ReturnedType> client, IDisposable subscription)>();
            _broadcaster = null;
            _errorManager = null;
            _timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, TickRate, TickRate);
            _dependencyContainer = new UnityContainer();
            RoutesAsync = new Dictionary<string, Func<IncomingType, IUnityContainer, CapstanEvent<IncomingType, ReturnedType>>>();
            Routes = new Dictionary<string, Func<IncomingType, IUnityContainer, CapstanEvent<IncomingType, ReturnedType>>>();
        }

        public IUnityContainer Dependencies { get; internal set; }
        public Broadcaster<IncomingType, ReturnedType> Broadcaster
        {
            get
            {
                if (_broadcaster == null)
                {
                    _broadcaster = BroadcasterFactory(Dependencies);
                    _broadcaster.InternalClients = ConvertClientsToReceivers;
                    _broadcaster.Messages.Subscribe(async (i) => await Push(i));
                }
                return _broadcaster;
            }
        }

        public ErrorManager<ReturnedType> ErrorManager
        {
            get
            {
                if (_errorManager == null)
                {
                    _errorManager = ErrorManagerFactory(Dependencies);
                    _errorManager.InternalSenders = ConvertClientsToDictionaryOfReceivers;
                }
                return _errorManager;
            }
        }

        public void Subscribe(Client<IncomingType, ReturnedType> client)
        {
            if (_clients.Any(i => i.client.Id == client.Id))
            {
                //User already subscribed
                return;
            }

            var subscription = client.Messages.Subscribe(async (i) => await Push(i));
            _clients.Add((client, subscription));
        }
        public void Unsubscribe(Client<IncomingType, ReturnedType> client)
        {
            var currentClient = _clients
                .Where(i => i.client == client)
                .SingleOrDefault();

            if (currentClient.client == null)
            {
                //No such user subscribed
                return;
            }

            currentClient.subscription.Dispose();
            _clients.Remove(currentClient);
        }
        public void Start()
        {
            _started = true;

            foreach (var activistFactory in ActivistFactories)
            {
                var activist = activistFactory(Dependencies);
                activist.Broadcaster = Broadcaster;
            }

            CapstanCycleEvent.Cycling = true;
        }
        public void Stop()
        {
            _started = false;
            CapstanCycleEvent.Cycling = false;
        }

        internal Dictionary<string, Func<IncomingType, IUnityContainer, CapstanEvent<IncomingType, ReturnedType>>> Routes { get; }
        internal Dictionary<string, Func<IncomingType, IUnityContainer, CapstanEvent<IncomingType, ReturnedType>>> RoutesAsync { get; }
        internal List<Func<IUnityContainer, Activist<IncomingType, ReturnedType>>> ActivistFactories { get; set; }
        internal Func<IUnityContainer, Broadcaster<IncomingType, ReturnedType>> BroadcasterFactory { get; set; }
        internal Func<IUnityContainer, ErrorManager<ReturnedType>> ErrorManagerFactory { get; set; }

        private async Task Push((string key, IncomingType value) @event)
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

        internal List<Client<IncomingType, ReturnedType>> ConvertClientsToReceivers()
        {
            return _clients
                .Select(i => i.client)
                .ToList();
        }

        private Dictionary<int, Receiver<ReturnedType>> ConvertClientsToDictionaryOfReceivers()
        {
            return _clients
                .Select(i => i.client)
                .ToDictionary(i => i.Id, i => (Receiver<ReturnedType>)i);
        }
    }
}
