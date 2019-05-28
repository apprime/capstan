namespace Capstan.Core
{
    public interface CapstanClient<TInput, TOutput> : CapstanSender<TInput>, CapstanReceiver<TOutput>
    {
    }
}
