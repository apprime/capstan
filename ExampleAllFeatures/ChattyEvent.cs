using System.Threading;
using System.Threading.Tasks;
using Capstan.Events;

namespace ExampleAllFeatures
{
    internal class ChattyEvent : CapstanEvent<StringMessage, string>
    {
        private StringMessage input;

        public ChattyEvent(StringMessage input)
        {
            this.input = input;
        }

        public async override void Process()
        {
            DoBoth();
        }

        public async override Task ProcessAsync()
        {
            DoBoth();
        }

        private void DoBoth()
        {
            Broadcaster.ToSender(input, "Yo listen up, here's the story");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "About a little guy that lives in a blue world");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "And all day and all night and everything he sees is just blue");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "Like him, inside and outside");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "Blue his house with a blue little window");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "And a blue Corvette");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "And everything is blue for him");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "And himself and everybody around");
            Thread.Sleep(100);
            Broadcaster.ToSender(input, "'Cause he ain't got nobody to listen to");
        }
    }
}