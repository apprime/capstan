namespace Capstan.Core
{
    public interface Receiver<TOutput>
    {
        void Receive(TOutput output);
    }
}
