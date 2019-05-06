using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
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

        private Dictionary<Type, Action<object>> _reactions;
        public Engine2 RegisterReactionary<T>(Func<IReactionary<T>> setHandler)
        {
            _reactions.Add(typeof(T), (evt) => setHandler().Subscribe((IRaiseEvent<T>)evt));
            return this;
        }

        private void RegisterActivists()
        {
            timer = new Timer(CapstanCycleEvent.OnTimerEvent, null, 1000, TickRate);
        }

        public Engine2 RegisterActivist(IActivist activist)
        {
            CapstanCycleEvent.RegisterActivist(activist);
            return this;
        }

        public Engine2 RegisterActivists(params IActivist[] activists)
        {
            foreach (var activist in activists) { CapstanCycleEvent.RegisterActivist(activist); }
            return this;
        }
    }

    public class a
    {
        private class activister : IActivist
        {
            public void Activate()
            {
                throw new NotImplementedException();
            }

            public bool Condition()
            {
                throw new NotImplementedException();
            }
        }

        public a()
        {
            var e = new Engine2()
                .RegisterActivists
                (
                    new activister(),
                    new activister()
                )
                .RegisterActivist(new activister())
                .RegisterReactionary(() => new ReactToLordZorkelbort());
        }
    }

    /// <summary>
    /// An event reactionary is a class that reacts
    /// to events raised by the system. They do not
    /// need to do anything specific, all we really 
    /// know is that they are triggered by events.
    /// </summary>
    public class ReactToLordZorkelbort : IReactionary<LordZorkelbortIsBack>
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

    public class LordZorkelbortHappens : IRaiseEvent<LordZorkelbortIsBack>
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
