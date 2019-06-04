using Capstan.Core;
using Capstan.Events;
using System;
using System.Collections.Generic;
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

             SetBroadcaster expect you to provide a broadcaster instance that inherits from Broadcaster<TOutput>.
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

            var capstan = CapstanBuilder<TestInput, string>
                .New()
                .SetBroadcaster(clients => new TestBroadcaster(clients))
                .SetErrorManager(clients => new TestErrorManager(clients))
                .RegisterActivist(new TestActivist())
                .RegisterDependencies(container =>
                {
                    container.RegisterType<IEnterpriseBusinessDependency, EnterpriseBusinessDependency>(new TransientLifetimeManager());
                })
                .ConfigRoute("Login", (evt, deps) => new TestEvent(evt))
                .ConfigRoutes
                (
                   new Dictionary<string, Func<TestInput, IUnityContainer, CapstanEvent>>
                   {
                       { "Logout", (evt, deps) => new TestEvent(evt)},
                       { "SomethingElse", (evt, deps) => new ChainOfCommandComplexEventOne(evt, deps)}
                   }
                )
                //Obviously don't use the same names for multiple routes.
                //The engine wont throw exceptions but additional Events are not added.
                //Also, Synchronous events wont be allowed for final version(Don't ask me when the final version is).
                .ConfigRouteAsync("LoginAsync", (evt, deps) => new ChainOfCommandComplexEventOne(evt, deps))
                .ConfigRoutesAsync
                (
                   new Dictionary<string, Func<TestInput, IUnityContainer, CapstanEvent>>
                   {
                       { "LogoutAsync", (evt, deps) => new TestEvent(evt)},
                       { "SomethingElseAsync", (evt, deps) => new ChainOfCommandComplexEventOne(evt, deps)}
                   }
                )
                .Build();

            capstan.Start();

            var user = new TestUser();
            capstan.Subscribe(user);

            user.Send(new TestInput("Demo!"));
        }
    }

    public class TestInput : CapstanMessage
    {
        public TestInput(string input)
        {
            Data = input;
        }
        public string Data;
        public int SenderId => throw new NotImplementedException();
    }

    public class TestActivist : Activist
    {
        private int counter = 1;
        public async Task Activate()
        {
            await Task.Factory.StartNew(() => throw new NotImplementedException());
        }

        public bool Condition()
        {
            //Increase one every time we check, return true every 10 times.
            return ++counter % 10 == 0;
        }
    }

    public class TestEvent : CapstanEvent
    {
        public TestEvent(TestInput something)
        {

        }

        public void Process()
        {
            throw new NotImplementedException();
        }

        public Task ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class TestComplexEvent : CapstanEvent
    {
        public TestComplexEvent(IEnterpriseBusinessDependency enterpriseBusinessDependency, ICapstanImplementation capstanImpmentation, string favouriteGoat)
        {
            //Whatever
        }

        public static int FavouriteGoatId { get; } = 3;

        public void Process()
        {
            throw new NotImplementedException();
        }

        public Task ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class TestUser : CapstanClient<TestInput, string>
    {
        public TestUser()
        {
        }

        public Subject<(string key, TestInput value)> Messages => new Subject<(string key, TestInput value)>();

        public int Id => GetHashCode();

        public void Receive(string output)
        {
            //Client takes string, parses it somehow if needed,
            //and does whatever it wants.
        }

        public void Send(TestInput input)
        {
            //Somewhere someone tells CapstanClient to cc.Send(stuff);
            //This results in a message that is pushed into the grinder.
            Messages.OnNext(("Logout", input));
        }
    }

    public class TestBroadcaster : Broadcaster<string>
    {
        private readonly List<CapstanReceiver<string>> innerDep;

        public TestBroadcaster(List<CapstanReceiver<string>> users)
        {
            innerDep = users;
        }

        public override IEnumerable<CapstanReceiver<string>> Clients
        {
            get => innerDep;
        }
    }

    public class TestErrorManager : ErrorManager<TestInput, string>
    {
        public TestErrorManager(List<CapstanClient<TestInput, string>> clients) : base(clients) { }

        public override string ParseError(Exception ex)
        {
            return ex.Message;
        }
    }

    public class ChainOfCommandComplexEventOne : CapstanEvent
    {
        private ChainOfCommandComplexEventTwo _testComplexEvent;

        public ChainOfCommandComplexEventOne(TestInput input, IUnityContainer container)
        {
            var ebd = container.Resolve<IEnterpriseBusinessDependency>();
            _testComplexEvent = new ChainOfCommandComplexEventTwo(input, ebd);
        }

        public void Process()
        {
            _testComplexEvent.Process();
        }

        public async Task ProcessAsync()
        {
            await _testComplexEvent.ProcessAsync();
        }
    }

    public class ChainOfCommandComplexEventTwo : CapstanEvent
    {
        private TestComplexEvent _testComplexEvent;

        public ChainOfCommandComplexEventTwo(TestInput input, IEnterpriseBusinessDependency ebd)
        {
            var cid = CapstanImplementationFactory.Manufacture(input);
            var goat = GoatFactory.GetGoat(TestComplexEvent.FavouriteGoatId);

            _testComplexEvent = new TestComplexEvent(ebd, cid, goat);
        }

        public void Process()
        {
            _testComplexEvent.Process();
        }

        public async Task ProcessAsync()
        {
            await _testComplexEvent.ProcessAsync();
        }
    }

    public interface IEnterpriseBusinessDependency
    {
        void GenerateXml(string text);
        byte[] ToExcel(object xmlDocument);
    }

    public class EnterpriseBusinessDependency : IEnterpriseBusinessDependency
    {
        public void GenerateXml(string text)
        {
            throw new NotImplementedException();
        }

        public byte[] ToExcel(object xmlDocument)
        {
            throw new NotImplementedException();
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
        public static ICapstanImplementation Manufacture(TestInput input)
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
