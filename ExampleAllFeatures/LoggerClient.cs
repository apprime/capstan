using Capstan.Core;
using System;
using System.Reactive.Subjects;

namespace ExampleAllFeatures
{
    //This is a quick and dirty way to keep track of which client the logger is.
    public static class Clients
    {
        public static int Logger = 1337;
    }

    public class LoggerClient : Client<StringMessage, string>
    {
        public LoggerClient(int id, string name)
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
            Console.WriteLine($"Logger is asked to log: {output}");
        }

        public void Send(string key, StringMessage input)
        {
            input.SenderId = Id;
            Messages.OnNext((key, input));
        }
    }
}
