namespace Capstan.Core
{
    public interface Client<TInput, TOutput> : Sender<TInput>, Receiver<TOutput>
    {
    }
}
