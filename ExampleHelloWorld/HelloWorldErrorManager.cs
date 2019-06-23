using Capstan.Core;
using System;
using System.Collections.Generic;

namespace ExampleHelloWorld
{
    public class HelloWorldErrorManager : Capstan.ErrorManager<string>
    {
        public override string ParseError(Exception ex)
        {
            return ex.Message;
        }
    }
}