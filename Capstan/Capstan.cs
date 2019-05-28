using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Capstan
{
    //Q: TInput likely needs information about origin.
    public class Capstan<TInput, TOutput>
    {
        private Timer timer;
        private const int TickRate = 1000;

        /// <summary>
        /// Use the CapstanBuilder class. 
        /// It will help you set everything up properly.
        /// </summary>
        internal Capstan()
        {
        }

        private async Task<IObserver<(string key, TInput value)>> Push((string key, TInput value) @event)
        {
            //try
            //{
            if (Routes.ContainsKey(@event.key))
            {
                //Create Event, call Sync
                Routes[@event.key](@event.value).Process();
            }

            if (RoutesAsync.ContainsKey(@event.key))
            {
                //Create Event, call Async
                await RoutesAsync[@event.key](@event.value).ProcessAsync();
            }
            //} 
            //catch(Exception ex)
            //{
            //    //Use Dictionary KeyNotFoundException to handle missing keys.
            //    //Use TInput to find client that provided this. Broadcast error message
            //}

            throw new ArgumentException(
                $"Incoming event with key {@event.key} does not exist as either an synchronous or asynchronous route." +
                $" Change the input key, or add a route to the engine during config, using either of the ConfigRoute methods.");
        }

        private List<(CapstanClient<TInput, TOutput> client, IDisposable subscription)> _clients =
            new List<(CapstanClient<TInput, TOutput> client, IDisposable subscription)>();
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

        internal Capstan<TInput, TOutput> RegisterActivists(params Activist[] activists)
        {
            foreach (var activist in activists) { CapstanCycleEvent.RegisterActivist(activist); }
            timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, TickRate, TickRate);
            return this;
        }

        internal Dictionary<string, Func<TInput, CapstanEvent>> Routes { get; } = new Dictionary<string, Func<TInput, CapstanEvent>>();
        internal Dictionary<string, Func<TInput, CapstanEvent>> RoutesAsync { get; } = new Dictionary<string, Func<TInput, CapstanEvent>>();

        public Broadcaster<TOutput> Broadcaster
        {
            get
            {
                if (_broadcaster == null)
                {
                    _broadcaster = BroadcasterFactory(_clients);
                }
                return _broadcaster;
            }
        }
        internal Func<List<(CapstanClient<TInput, TOutput> client, IDisposable subscription)>, Broadcaster<TOutput>> BroadcasterFactory { get; set; }
        private Broadcaster<TOutput> _broadcaster = null;
    }
}
