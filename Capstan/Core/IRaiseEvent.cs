using System;

namespace Capstan.Core
{
    public interface IRaiseEvent<T>
    {
        event EventHandler<T> OnEvent;
    }
}
