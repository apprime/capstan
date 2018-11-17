namespace Capstan.Core
{
    public interface IReactionary<T>
    {
        void Subscribe(IRaiseEvent<T> @event);
        void Handler(object e, T eventData);
    }
}
