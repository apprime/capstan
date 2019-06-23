using Capstan.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Capstan
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
    public class Broadcaster<IncomingType, ReturnedType> where IncomingType : Message
    {
        internal Func<List<Client<IncomingType, ReturnedType>>> InternalClients { private get;  set; }
        internal Subject<(string key, IncomingType payload)> Messages { get; private set; }

        public Broadcaster()
        {
            Messages = new Subject<(string key, IncomingType payload)>();
            InternalClients = () => new List<Client<IncomingType, ReturnedType>>();
        }

        /// <summary>
        /// This method pushes 
        /// </summary>
        /// <param name="payload"></param>
        public virtual void Broadcast(ReturnedType payload)
        {
            foreach (var client in Clients.Where(_filter)){ client.Receive(payload); }
            _filter = (i) => true;
        }

        private Predicate<Client<IncomingType, ReturnedType>> _filter = (i) => true;
        public virtual Broadcaster<IncomingType, ReturnedType> Filter(Predicate<Client<IncomingType, ReturnedType>> filter)
        {
            _filter = filter;
            return this;
        }

        public virtual void ToSender(IncomingType input, ReturnedType payload)
        {
            Clients.Single(i => i.Id == input.SenderId).Receive(payload);
        }

        public virtual void ToCapstan(string messageKey, IncomingType newMessage)
        {
            Messages.OnNext((messageKey, newMessage));
        }

        /// <summary>
        /// Inside of the broadcaster there should be some way to know
        /// which clients are connected. (Inject a dependency in your concrete implementation)
        /// </summary>
        public virtual IEnumerable<Client<IncomingType, ReturnedType>> Clients => InternalClients();
    }
}
