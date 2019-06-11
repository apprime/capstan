using Capstan.Core;
using System;
using System.Collections.Generic;

namespace ExampleHelloWorld
{
    public class HelloWorldErrorManager : Capstan.ErrorManager<string>
    {
        private readonly Dictionary<int, Receiver<string>> clients;

        protected override Dictionary<int, Receiver<string>> GetClients()
        {
            return clients;
        }

        public override string ParseError(Exception ex)
        {
            return ex.Message;
        }
    }
}