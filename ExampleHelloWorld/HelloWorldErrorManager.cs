using Capstan.Core;
using System;
using System.Collections.Generic;

namespace ExampleHelloWorld
{
    public class HelloWorldErrorManager : Capstan.ErrorManager<string>
    {
        public HelloWorldErrorManager(Dictionary<int, Receiver<string>> clients)
        {
            Clients = clients; 
        }

        protected override Dictionary<int, Receiver<string>> Clients { get; set; }

        public override string ParseError(Exception ex)
        {
            return ex.Message;
        }
    }
}