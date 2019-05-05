using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Capstan
{
    public class Engine
    {
        //TODO: Setup Engine with a builder pattern.
        /*
            Register activists, builder.AddActivist(IActivist)?
            builder.AddReactionary()?
        */



        private BatchQueue<CapstanEvent> _eventList;

        //TODO: We want to be able to set these values in config I suppose?
        public bool run = false;
        private const int MaxQueueSize = 1;
        public AutoResetEvent _loopTimer;

        private Engine()
        {
            _eventList = new BatchQueue<CapstanEvent>();
            _loopTimer = new AutoResetEvent(false);
        }

        public Engine Create()
        {
            var e = new Engine();
            RegisterEvents(Assembly.GetCallingAssembly());
            return e;
        }

        /// <summary>
        /// This Dictionary contains all categories of Event, including the empty category.
        /// For each category there is another dictionary of names for the events and a func that will 
        /// initiate new new instance of the Event class.
        /// </summary>
        private Dictionary<string, Dictionary<string, Func<string[], CapstanEvent>>> categories;

        private void AddEventAttribute(string category, string name, Func<string[], CapstanEvent> instanceGetter)
        {
            if (!categories.ContainsKey(category))
            {
                categories.Add(category, new Dictionary<string, Func<string[], CapstanEvent>>());
            }

            categories[category].Add(name, instanceGetter);
        }

        /// <summary>
        /// The engine will find all Events that have been added to the callingAssembly
        /// Then register those in a registry for use when engine is fed events in string form.
        /// </summary>
        /// <param name="callingAssembly"></param>
        private void RegisterEvents(Assembly callingAssembly)
        {
            foreach (Type type in callingAssembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<EventAttribute>();
                if (attr != null)
                {
                    var paramType = new Type[] { typeof(object[]) };
                    Func<string[], CapstanEvent> func = (parameters) => (CapstanEvent)type.GetConstructor(paramType).Invoke(parameters);
                    AddEventAttribute(attr.Category, attr.Name, func);
                }
            }
        }

        private void Tick()
        {
                var eventChunk = _eventList.DequeueChunk(10);
                //TODO: EventCulling!
                // 1. Remove duplicate events
                // 2. Remove events that are "too late", i.e. acting on something that disappears earlier in this chunk (How?)
                Task.Factory.StartNew(() => ProcessBatch(eventChunk));
        }

        private async Task ProcessBatch(IEnumerable<CapstanEvent> batch)
        {
            await Task.WhenAll(batch.Select(i => i.Process()));
            return;
        }

        public void Push(CapstanEvent e)
        {
            _eventList.Enqueue(e);
            //We could use an event to trigger if the queue grows too big, but since we know when we are adding to it, I don't think we need it.
            //TODO:  monitoring hook that can alert when overflow is beginnning
            //TODO:  autoscaling of potential cloud server (I know, super important in early access super pre-alpha)
            PreventOverflow();
        }

        private void PreventOverflow()
        {
            if (_eventList.Count >= MaxQueueSize)
            {
                Tick();
                _loopTimer.Reset(); //Don't double dip with events.
            }
        }
    }
}
