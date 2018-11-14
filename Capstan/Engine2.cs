using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstan
{
    public class Engine2
    {
        public async Task<IQueryResult<T>> PushQuery<T>(Query<T> query)
        {
            return await query.Process();
        }

        public async Task<ICommandResult> PushCommand(Command command)
        {
            return await command.Process();
        }

        public void RegisterSubscriber<T>(IRaiseEvent<T> subscriber)
        {

        }


    }

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
    /// When Capstan is created, the object has an "AddSubscriber" method
    /// This method will allow you to register any listeners for server events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Broadcaster<T>
    {
        public delegate void ServerPushEventHandler(IPayload<T> payload, Predicate<ISubscriber<T>> selector);
        public ServerPushEventHandler handler;
        public Broadcaster()
        {
            handler = new ServerPushEventHandler(Broadcast);
        }

        private readonly IEnumerable<ISubscriber<T>> _subscribers;

        public void Broadcast(IPayload<T> payload, Predicate<ISubscriber<T>> selector)
        {
            foreach (var sub in _subscribers.Where(selector)) { sub.Receive(payload); }
        }
    }

    public interface ISubscriber<T>
    {
        Task Receive(IPayload<T> payload);
    }

    public class EventReactionary
    {

    }


    public class CustomReact : EventReactionary
    {
        public EventHandler<LordZorkelbortIsBack> Handler;
        public void Subscribe<T>(IRaiseEvent<T> @event, EventHandler<T> handler)
        {
            @event.OnEvent += new EventHandler<T>(HandleZorkelBort);
        }
        public CustomReact(IRaiseEvent<LordZorkelbortIsBack> @event)
        {
            Subscribe(@event, Handler);
            //new CustomReact(
        }

        //public delegate EventHandler<LordZorkelbortIsBack> Subscriber;

        private delegate EventHandler<T>(LordZorkelbortIsBack args) HandleZorkelBort;

        {
            var x = 5;
            x = x < args.Important ? 6 : 4;
            return new EventHandler<T>();
        }
    }

    //public delegate void EventHandler<T>

    public interface IListenTo<T>
    {
        T Handler { get; }
    }

    public interface IRaiseEvent<T>
    {
        event EventHandler<T> OnEvent;
    }

    public class CustomEventHappens : IRaiseEvent<LordZorkelbortIsBack>
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
