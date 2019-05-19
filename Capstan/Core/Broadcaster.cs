using Capstan.Events;
using System;
using System.Collections.Generic;

namespace Capstan.Core
{
    /// <summary>
    /// A broadcaster is a class that converts Captstan Event information
    /// into something that can be handed over to a connected user.
    /// It also knows how to handle all users and filter them according
    /// to which users are supposed to know about this event.
    /// Last but not least it can also actually carry out the transmission,
    /// however, one by one.
    /// </summary>
    public abstract class Broadcaster<TPayload, TUser>
    {
        public Broadcaster()
        { }

        /// <summary>
        /// This method pushes 
        /// </summary>
        /// <param name="payload"></param>
        public void Broadcast(TPayload payload)
        {
            Set(payload);
            foreach (var recipient in FilterUsers(payload)) { Send(recipient); }
            Unset();
        }

        /// <summary>
        /// This method describes how a mapped payload is 
        /// pushed to a user.
        /// </summary>
        internal abstract void Send(TUser user);

        /// <summary>
        /// Inside of a payload should be some way to know
        /// how to find information about which users to broadcast to.
        /// </summary>
        protected abstract IEnumerable<TUser> FilterUsers(TPayload payload);

        /// <summary>
        /// A broadcaster must know how to turn the Payload
        /// object into something we can push to a users client.
        /// </summary>
        protected abstract Broadcaster<TPayload, TUser> Set(TPayload payload);

        /// <summary>
        /// A broadcaster must know how to turn the EventResult
        /// object into something we can push to a users client.
        /// </summary>
        internal abstract Broadcaster<TPayload, TUser> Set(IEventResult payload);

        /// <summary>
        /// Unsets the current to be broadcast Users.
        /// </summary>
        protected abstract void Unset();
    }
}
