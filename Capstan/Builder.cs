using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using Unity;

namespace Capstan
{
    public class Builder<TInput,TOutput> where TInput : Message 
    {
        private Capstan<TInput, TOutput> _instance;
        private IUnityContainer _dependencyContainer;

        public static Builder<TInput, TOutput> New()
        {
            var builder = new Builder<TInput, TOutput>();
            builder._dependencyContainer = new UnityContainer();
            builder._instance = new Capstan<TInput, TOutput>();
            return builder;
        }

        public IUnityContainer Dependencies { get; }
        public Builder<TInput, TOutput> RegisterDependencies(Action<IUnityContainer> action)
        {
            action(Dependencies);
            return this;
        }

        public Builder<TInput, TOutput> SetBroadcaster(Func<List<Client<TInput, TOutput>>, IUnityContainer, Broadcaster<TInput, TOutput>> factory)
        {
            _instance.BroadcasterFactory = factory;
            return this;
        }
        
        public Builder<TInput, TOutput> SetErrorManager(Func<Dictionary<int, Receiver<TOutput>>, IUnityContainer, ErrorManager<TOutput>> factory)
        {
            _instance.ErrorManagerFactory = factory;
            return this;
        }

        public Builder<TInput, TOutput> RegisterActivist(Activist activist)
        {
            //TODO: Take factory method here. 
            // We want to be able to kill and regenerate activists.
            // Also, we need to be able to inject dependencies into them.
            CapstanCycleEvent.RegisterActivist(activist);
            return this;
        }

        public Builder<TInput, TOutput> AddRoute(string key, Func<TInput, IUnityContainer, CapstanEvent<TInput, TOutput>> eventFactory)
        {
            _instance.Routes.TryAdd(key, eventFactory);
            return this;
        }

        public Builder<TInput, TOutput> AddRouteAsync(string key, Func<TInput, IUnityContainer, CapstanEvent<TInput, TOutput>> eventFactory)
        {
            _instance.RoutesAsync.TryAdd(key, eventFactory);
            return this;
        }

        public Capstan<TInput, TOutput> Build()
        {
            if (_instance.BroadcasterFactory == null)
            {
                throw new Exception("Broadcaster has not been provided a factory method and cannot be created.");
            }

            if (_instance.ErrorManagerFactory == null)
            {
                throw new Exception("ErrorManager has not been provided a factory method and cannot be created.");
            }

            if(_instance.Routes.Count == 0 && _instance.RoutesAsync.Count == 0)
            {
                throw new Exception("This capstan instance cannot be built because it has no routes assigned");
            }

            _instance.Dependencies = _dependencyContainer;

            return _instance;
        }
    }
}
