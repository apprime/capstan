using Capstan;
using Capstan.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAllFeatures
{
    partial class Program
    {
        public class PeoplePoker<IncomingType, ReturnedType>
            : Activist<IncomingType, ReturnedType> where IncomingType : Message
        {
            internal Broadcaster<StringMessage, string> Broadcaster { set; private get; }
            private int counter = 1;

            public override async Task Activate()
            {
                await Task.Factory.StartNew(() =>
                {
                    var rng = new Random().Next(0, Broadcaster.Clients.Count() - 1);
                    var client = Broadcaster.Clients.Skip(rng).Single();
                    Broadcaster
                    .Filter(i => i.Id == client.Id)
                    .Broadcast("Hello, I am the people poker and you have been randomly selected to receive a poke.");
                });
            }

            public override bool Condition()
            {
                //Increase one every time we check, return true every 10 times.
                return ++counter % 10 == 0;
            }
        }
    }
}
