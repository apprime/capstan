using Capstan.Core;
using System;
using System.Reactive.Subjects;

namespace ExampleHelloWorld
{
    internal class HelloWorldClient<T1, T2> : Client<HelloWorldInput, string>
    {
        public Subject<(string key, HelloWorldInput value)> Messages { get; set; }
        public int Id { get; private set; }

        public HelloWorldClient(int newId)
        {
            Messages = new Subject<(string key, HelloWorldInput value)>();
            Id = newId;
        }

        public void Receive(string output)
        {
            Console.WriteLine($"Capstan says: {output}");
        }

        public void Send(string key, HelloWorldInput input)
        {
            input.SenderId = Id;
            Messages.OnNext((key, input));
        }
    }
}