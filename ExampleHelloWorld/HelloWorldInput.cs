using Capstan;

namespace ExampleHelloWorld
{
    public class HelloWorldInput : Message
    {
        public HelloWorldInput(string newValue)
        {
            OurValue = newValue;
        }
        public string OurValue { get; set; }
        public int SenderId { get; set; }
    }
}
