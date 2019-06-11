using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using Unity;

namespace Capstan
{
    public class Builder<IncomingType,ReturnedType> where IncomingType : Message 
    {
        private Capstan<IncomingType, ReturnedType> _instance;
        private IUnityContainer _dependencyContainer;

        public static Builder<IncomingType, ReturnedType> New()
        {
            var builder = new Builder<IncomingType, ReturnedType>();
            builder._dependencyContainer = new UnityContainer();
            builder._instance = new Capstan<IncomingType, ReturnedType>();
            return builder;
        }

        public IUnityContainer Dependencies { get; }
        public Builder<IncomingType, ReturnedType> RegisterDependencies(Action<IUnityContainer> action)
        {
            action(Dependencies);
            return this;
        }

        public Builder<IncomingType, ReturnedType> SetBroadcaster(Func<IUnityContainer, Broadcaster<IncomingType, ReturnedType>> factory)
        {
            _instance.BroadcasterFactory = factory;
            return this;
        }
        
        public Builder<IncomingType, ReturnedType> SetErrorManager(Func<IUnityContainer, ErrorManager<ReturnedType>> factory)
        {
            _instance.ErrorManagerFactory = factory;
            return this;
        }

        private List<Func<IUnityContainer, Activist<IncomingType, ReturnedType>>> _activistFactories = new List<Func<IUnityContainer, Activist<IncomingType, ReturnedType>>>();
        public Builder<IncomingType, ReturnedType> RegisterActivist(Func<IUnityContainer, Activist<IncomingType, ReturnedType>> activistFactory)
        {
            _activistFactories.Add(activistFactory);
            return this;
        }

        public Builder<IncomingType, ReturnedType> AddRoute(string key, Func<IncomingType, IUnityContainer, CapstanEvent<IncomingType, ReturnedType>> eventFactory)
        {
            _instance.Routes.TryAdd(key, eventFactory);
            return this;
        }

        public Builder<IncomingType, ReturnedType> AddRouteAsync(string key, Func<IncomingType, IUnityContainer, CapstanEvent<IncomingType, ReturnedType>> eventFactory)
        {
            _instance.RoutesAsync.TryAdd(key, eventFactory);
            return this;
        }

        public Capstan<IncomingType, ReturnedType> Build()
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
            _instance.ActivistFactories = _activistFactories;

            return _instance;
        }
    }
}
