using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capstan
{
    public class Engine2
    {
        private const int TickRate = 100;
        private Timer timer;

        public async Task<IEventResult> Push(CapstanEvent @event)
        {
            return await @event.Process();
        }

        public void RegisterSubscriber<T>(IRaiseEvent<T> @event, IReactionary<T> reactionary)
        {

        }


        private List<IActivist> _activists;
        public void RegisterActivists()
        {


            timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, 1000, TickRate);
        }

      
    }

    public interface ISubscriber<T>
    {
        Task Receive(T payload);
    }



    /// <summary>
    /// An event reactionary is a class that reacts
    /// to events raised by the system. They do not
    /// need to do anything specific, all we really 
    /// know is that they are triggered by events.
    /// </summary>
    public class CustomReact : IReactionary<LordZorkelbortIsBack>
    {
        public void Subscribe(IRaiseEvent<LordZorkelbortIsBack> @event)
        {
            @event.OnEvent += Handler;
        }
        
        public void Handler(object e, LordZorkelbortIsBack args)
        {
            // He is too strong
            // There is nothing that can handle the lord!
        }
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
