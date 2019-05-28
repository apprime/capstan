using Capstan.Events;

namespace Capstan.Core
{
    public interface CapstanReceiver<TOutput>
    {
        void Receive(TOutput output);
    }
}
