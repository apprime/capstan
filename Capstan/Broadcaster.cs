using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;

namespace Capstan
{
    /// <summary>
    /// All broadcasters subscribe to server generated events.
    /// Their job is to listen to event, pick up payload and 
    /// push the Typed payload to all subscribers.
    /// 
    /// Example: Every 1 hours, server generates a zone wide emote
    /// "Lord Zorkelbort says: 'I have risen again!'"
    /// 
    /// The passed along selector will be used to find all subscribers 
    /// in the correct zone, then push the message event to them.
    /// When the Payload reaches the subscribing Receive function, 
    /// broadcaster is done.
    /// 
    /// How to use:
    /// Create a project, load Capstan as a service in that project
    /// When Capstan is created, the object has an "AddBroadcaster" method
    /// This method will allow you to register any listeners for broadcasting events.
    /// 
    /// Recommended syntax: myBroadcaster.Set(payload).Broadcast();
    /// </summary>
    public abstract class Broadcaster<TPayload> : IReactionary<TPayload>
    {
        public delegate void EventHandler(object e, TPayload @event);
        public EventHandler handler;

        public Broadcaster()
        {
            //TODO: Inject a _userService here. _userService = userService;
            //TODO: We need an object type for users that is not "object"
            //TODO: Do not filter for each request, update filter only when users list changes. 
        }

        private object _userService = null;
        private object _mappedValue = null;
        //private IEnumerable<object> Users => FilterUsers(._userService.all());

        public void Broadcast()
        {
            //foreach (var user in Users) { sub.SOMEMETHOD(_mappedValue); }
        }

        public void Subscribe(IRaiseEvent<TPayload> @event)
        {
            handler = new EventHandler(Handler);
        }

        /// <summary>
        /// A broadcaster must know which users out of all users,
        /// that will receive this broadcast.
        /// </summary>
        protected abstract IEnumerable<object> FilterUsers(IEnumerable<object> allUsers);

        /// <summary>
        /// A broadcaster must know how to turn the Payload
        /// object into something we can push to a users client.
        /// </summary>
        /// <remarks>Make sure this method returns the object itself (return this)</remarks>
        protected abstract Broadcaster<TPayload> Set(TPayload payload);

        public abstract void Handler(object e, TPayload eventData);
    }
}
