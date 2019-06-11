using Capstan.Core;
using System;
using System.Reactive.Subjects;

namespace ExampleAllFeatures
{
    public class GenericClient : Client<StringMessage, string>
    {
        public GenericClient(int id, string name)
        {
            Messages = new Subject<(string key, StringMessage value)>();
            Id = id;
            Name = name;
        }

        public Subject<(string key, StringMessage value)> Messages { get; }

        public int Id { get; }
        public string Name { get; }

        public void Receive(string output)
        {
            Console.WriteLine($"{Name} received: {output}");
        }

        public void Send(string key, StringMessage input)
        {
            input.SenderId = Id;
            Messages.OnNext((key, input));
        }
    }
}
