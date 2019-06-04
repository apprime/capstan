﻿using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using Unity;

namespace Capstan
{
    public class CapstanBuilder<TInput,TOutput> where TInput : CapstanMessage 
    {
        private Capstan<TInput, TOutput> _instance;
        private IUnityContainer _dependencyContainer;

        public static CapstanBuilder<TInput, TOutput> New()
        {
            var builder = new CapstanBuilder<TInput, TOutput>();
            builder._dependencyContainer = new UnityContainer();
            builder._instance = new Capstan<TInput, TOutput>();
            return builder;
        }

        public IUnityContainer Dependencies { get; }
        public CapstanBuilder<TInput, TOutput> RegisterDependencies(Action<IUnityContainer> action)
        {
            action(Dependencies);
            return this;
        }

        public CapstanBuilder<TInput, TOutput> Begin(Capstan<TInput, TOutput> capstan)
        {
            _instance = capstan;
            return this;
        }

        public CapstanBuilder<TInput, TOutput> SetBroadcaster(Func<List<CapstanReceiver<TOutput>>, Broadcaster<TOutput>> factory)
        {
            _instance.BroadcasterFactory = factory;
            return this;
        }
        
        public CapstanBuilder<TInput, TOutput> SetErrorManager(Func<List<CapstanClient<TInput, TOutput>>, ErrorManager<TInput, TOutput>> factory)
        {
            _instance.ErrorManagerFactory = factory;
            return this;
        }

        public CapstanBuilder<TInput, TOutput> RegisterActivist(Activist activist)
        {
            //TODO: Take factory method here. 
            // We want to be able to kill and regenerate activists.
            // Also, we need to be able to inject dependencies into them.
            CapstanCycleEvent.RegisterActivist(activist);
            return this;
        }

        public CapstanBuilder<TInput, TOutput> ConfigRoute(string key, Func<TInput, IUnityContainer, CapstanEvent> eventFactory)
        {
            _instance.Routes.TryAdd(key, eventFactory);
            return this;
        }

        public CapstanBuilder<TInput, TOutput> ConfigRoutes(Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _instance.Routes.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        public CapstanBuilder<TInput, TOutput> ConfigRouteAsync(string key, Func<TInput, IUnityContainer, CapstanEvent> eventFactory)
        {
            _instance.RoutesAsync.TryAdd(key, eventFactory);
            return this;
        }

        public CapstanBuilder<TInput, TOutput> ConfigRoutesAsync(Dictionary<string, Func<TInput, IUnityContainer, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _instance.RoutesAsync.TryAdd(route.Key, route.Value);
            }

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
            _instance.RegisterActivists();

            return _instance;
        }
    }
}
