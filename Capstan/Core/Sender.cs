using System.Reactive.Subjects;

namespace Capstan.Core
{
    public interface Sender<TInput>
    {
        Subject<(string key, TInput value)> Messages { get; }
        int Id { get; }
        void Send(string key, TInput input);
    }
}
