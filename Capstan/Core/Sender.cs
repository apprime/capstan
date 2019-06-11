using System.Reactive.Subjects;

namespace Capstan.Core
{
    public interface Sender<IncomingType>
    {
        Subject<(string key, IncomingType value)> Messages { get; }
        int Id { get; }
        void Send(string key, IncomingType input);
    }
}
