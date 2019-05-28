namespace Capstan.Core
{
    public interface CapstanSender<TInput>
    {
        void Send(TInput input);
    }
}
