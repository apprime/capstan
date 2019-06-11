using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Capstan
{
    public class BasicExample
    {
        public void TestIntMain()
        {
            /*
             Scenario: 
             Our system can gather information from any number of sources.
             This information comes to our Engine class in the form of the input parameter.
             This input can be of any type(same as the type give to our Engine when we create it)
             In this example, it is an array of strings.
             The key is used to find the correct event to map this data to, the value is the information
             we want to send into the event when we create it. The client must know the name of the event
             when pushing things into the engine.

             New is a static method that creates a new Capstan object and returns a builder for it.

             SetBroadcaster expect you to provide a broadcaster instance that inherits from Broadcaster<ReturnedType>.
             It will allow your connected users to be notified when things happen.

             Set ErrorManager expects you to provide an object from a class that inherits from the ErrorManager.
             This class is responsible for sending errors back to the original source of an event.

             Registering activists is done when you have modules in your code that needs to run either on a schedule,
             such as a batch job, or due to some kind of state in the application. 

             RegisterDependecies only wraps a IUnityContainer object inside of a method. This allows us to register
             all of our dependencies in one go and carry on with the builder. Unity is a Microsoft dependency injection
             framework and it is used here exactly as how you would use it anywhere else, but inside of an Action.

             We then specify a number of routes by string name. There is currently no option to use other 
             types of keys than strings. For each key, we provide a factory method that will create the event for us.
             This factory method also provides the Unity Container that was regiesterd earlier.
             For such dependecy resolution, look at the ComplexEvent that is regiesterd on some routes. It uses a chain of
             command structure for handling dependencies of various kinds before the event itself is processed.

             We can either add a single route at a time, or send in an entire dictionary of routes. It comes down to preference.
             The type of the Event must implement from the CapstanEvent interface. Now we have set up the infrastructure required
             to turn (hopefully) all arrays of strings into Events, with data properly provided to them. The Engine can now use the 
             Process/ProcessAsync-methods as soon as possible and make the event happen.

             Once your events end up doing something that clients need to be notified of, get a hold of your broadcaster and broadcast 
             some information - a string in this example.
             */

                .AddRouteAsync("LoginAsync", (evt, deps) => new ChainOfCommandComplexEventOne(evt, deps))
                .AddRouteAsync("LogoutAsync", (evt, deps) => new TestEvent(evt))
                .Build();

            capstan.Start();

        }
    }





    public class TestComplexEvent : CapstanEvent<TesIncomingType, string>
    {
        public TestComplexEvent(IEnterpriseBusinessDependency enterpriseBusinessDependency, ICapstanImplementation capstanImpmentation, string favouriteGoat)
        {
            //Whatever
        }

        public static int FavouriteGoatId { get; } = 3;

        public override void Process()
        {
            Broadcaster.Broadcast("This message is sent to all connected clients");
            throw new NotImplementedException();
        }

        public override Task ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }



    public class ChainOfCommandComplexEventOne : CapstanEvent<TesIncomingType, string>
    {
        private ChainOfCommandComplexEventTwo _testComplexEvent;

        public ChainOfCommandComplexEventOne(TesIncomingType input, IUnityContainer container)
        {
            var ebd = container.Resolve<IEnterpriseBusinessDependency>();
            _testComplexEvent = new ChainOfCommandComplexEventTwo(input, ebd);
        }

        public override void Process()
        {
            _testComplexEvent.Process();
        }

        public override async Task ProcessAsync()
        {
            await _testComplexEvent.ProcessAsync();
        }
    }

    public class ChainOfCommandComplexEventTwo : CapstanEvent<TesIncomingType, string>
    {
        private TestComplexEvent _testComplexEvent;

        public ChainOfCommandComplexEventTwo(TesIncomingType input, IEnterpriseBusinessDependency ebd)
        {
            var cid = CapstanImplementationFactory.Manufacture(input);
            var goat = GoatFactory.GetGoat(TestComplexEvent.FavouriteGoatId);

            _testComplexEvent = new TestComplexEvent(ebd, cid, goat);
        }

        public override void Process()
        {
            _testComplexEvent.Process();
        }

        public override async Task ProcessAsync()
        {
            await _testComplexEvent.ProcessAsync();
        }
    }

  

    public static class GoatFactory
    {
        public static string GetGoat(int goatIndex)
        {
            if (goatIndex == 3)
            {
                return "Big Ram Bruce";
            }

            return "Small Ram Bruce";
        }
    }

    public static class CapstanImplementationFactory
    {
        public static ICapstanImplementation Manufacture(TesIncomingType input)
        {
            if (input.SenderId == 123)
            {
                return new DoStuffA();
            }
            else
            {
                return new DoStuffB();
            }
        }
    }

    public interface ICapstanImplementation
    {
        void DoStuff();
    }

    public class DoStuffA : ICapstanImplementation
    {
        public void DoStuff()
        {
            throw new NotImplementedException();
        }
    }

    public class DoStuffB : ICapstanImplementation
    {
        public void DoStuff()
        {
            throw new NotImplementedException();
        }
    }


}
