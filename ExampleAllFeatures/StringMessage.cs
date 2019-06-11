using Capstan;

namespace ExampleAllFeatures
{
    public class StringMessage : Message
    {
        public StringMessage(string body)
        {
            MessageBody = body;
        }

        public int SenderId { get; set; }
        public string MessageBody { get; set; }
        public string SenderName { get; set; }
        public ClientType SenderType { get; set; }
    }
}