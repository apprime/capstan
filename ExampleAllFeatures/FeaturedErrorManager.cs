using Capstan;
using System;

namespace ExampleAllFeatures
{
    partial class Program
    {
        public class FeaturedErrorManager : ErrorManager<string>
        {
            public FeaturedErrorManager() { }

            public override string ParseError(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
