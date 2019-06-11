namespace Capstan.Core
{
    public interface Receiver<ReturnedType>
    {
        void Receive(ReturnedType output);
    }
}
