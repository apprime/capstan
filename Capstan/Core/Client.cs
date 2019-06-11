namespace Capstan.Core
{
    public interface Client<IncomingType, ReturnedType> : Sender<IncomingType>, Receiver<ReturnedType>
    {
    }
}
