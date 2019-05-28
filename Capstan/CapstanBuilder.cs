using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;

namespace Capstan
{
    public class CapstanBuilder<TInput,TOutput>
    {
        private Capstan<TInput, TOutput> _instance;

        public static CapstanBuilder<TInput, TOutput> New()
        {
            var builder = new CapstanBuilder<TInput, TOutput>();
            builder._instance = new Capstan<TInput, TOutput>();
            return builder;
        }

        public CapstanBuilder<TInput, TOutput> Begin(Capstan<TInput, TOutput> capstan)
        {
            _instance = capstan;
            return this;
        }

        public CapstanBuilder<TInput, TOutput> SetBroadcaster(Func<List<CapstanClient<TInput, TOutput>>, Broadcaster<TOutput>> factory)
        {
            _instance.BroadcasterFactory = factory;
            return this;
        }

        public CapstanBuilder<TInput, TOutput> RegisterActivist(Core.Activist activist)
        {
            //TODO: Take factory method here. 
            // We want to be able to kill and regenerate activists.
            CapstanCycleEvent.RegisterActivist(activist);
            return this;
        }

        public CapstanBuilder<TInput, TOutput> ConfigRoute(string key, Func<TInput, CapstanEvent> eventFactory)
        {
            _instance.Routes.TryAdd(key, eventFactory);
            return this;
        }
        public CapstanBuilder<TInput, TOutput> ConfigRoutes(Dictionary<string, Func<TInput, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _instance.Routes.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        public CapstanBuilder<TInput, TOutput> ConfigRouteAsync(string key, Func<TInput, CapstanEvent> eventFactory)
        {
            _instance.RoutesAsync.TryAdd(key, eventFactory);
            return this;
        }
        public CapstanBuilder<TInput, TOutput> ConfigRoutesAsync(Dictionary<string, Func<TInput, CapstanEvent>> routes)
        {
            foreach (var route in routes)
            {
                _instance.RoutesAsync.TryAdd(route.Key, route.Value);
            }

            return this;
        }

        public void Build()
        {
            if (_instance.BroadcasterFactory == null)
            {
                throw new Exception("Broadcaster has not been provided a factory method and cannot be created.");
            }

            _instance.RegisterActivists();
        }
    }
}
