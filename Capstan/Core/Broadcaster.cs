using System;
using System.Collections.Generic;

namespace Capstan.Core
{
    /// <summary>
    /// A broadcaster is a class that converts information from Captstan Events 
    /// into something that can be handed over to a connected client.
    /// 
    /// CapstanReceiver is one of two interfaces that must be implemented by 
    /// a client class that can be subscribed to capstan.
    /// 
    /// Send to all Clients where Value is > 5
    /// myBroadCaster.Filter(i => i.Value > 5).Broadcast(myPayload); 
    /// 
    /// Send to all Clients:
    /// myBroadCaster.Broadcast(myPayload);
    /// 
    /// --Recommendation--
    /// Create static Predicates that can be accessed globally for
    /// consistent filtering among clients that should be interested.
    /// 
    /// </summary>
    public abstract class Broadcaster<TInput, TOutput>
    { 
        public Broadcaster()
        { }

        /// <summary>
        /// This method pushes 
        /// </summary>
        /// <param name="payload"></param>
        public void Broadcast(TOutput payload)
        {
            foreach (var client in Clients.Where(_filter)){ client.Receive(payload); }
            _filter = (i) => true;
        }

        private Predicate<Client<TInput, TOutput>> _filter = (i) => true;
        public Broadcaster<TInput, TOutput> Filter(Predicate<Client<TInput, TOutput>> filter)
        {
            _filter = filter;
            return this;
        }

        /// <summary>
        /// Inside of the broadcaster there should be some way to know
        /// which clients are connected. (Inject a dependency in your concrete implementation)
        /// </summary>
        public abstract IEnumerable<Client<TInput, TOutput>> Clients { get; }
    }
}
