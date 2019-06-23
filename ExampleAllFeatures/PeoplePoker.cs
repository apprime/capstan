using Capstan;
using Capstan.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAllFeatures
{
    internal partial class Program
    {
        public class PeoplePoker<IncomingType, ReturnedType>
            : Activist<IncomingType, string> where IncomingType : Message
        {
            private int counter = 1;
            private Random rnd = new Random();
            public override async Task Activate()
            {
                await Task.Factory.StartNew(() =>
                {
                    var rng = rnd.Next(0, Broadcaster.Clients.Count());
                    var client = Broadcaster.Clients.Skip(rng).First();
                    Broadcaster
                    .Filter(i => i.Id == client.Id)
                    .Broadcast("Hello, I am the people poker and you have been randomly selected to receive a poke.");
                });
            }   

            public override bool Condition()
            {
                //Increase one every time we check, return true every 10 times.
                return ++counter % 5 == 0;
            }
        }
    }
}
