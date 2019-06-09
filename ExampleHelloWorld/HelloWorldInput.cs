using Capstan;

namespace ExampleHelloWorld
{
    class HelloWorldInput : Message
    {
        public HelloWorldInput(string newValue)
        {
            OurValue = newValue;
        }
        public string OurValue { get; set; }
        public int SenderId { get; set; }
    }
}
