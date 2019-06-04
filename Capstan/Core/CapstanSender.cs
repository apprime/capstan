using System.Reactive.Subjects;

namespace Capstan.Core
{
    public interface CapstanSender<TInput>
    {
        Subject<(string key, TInput value)> Messages { get; }
        int Id { get;  }
        void Send(TInput input);
    }
}
