﻿namespace Capstan.Events
{
    public interface ICommandResult
    {
        EventResolutionType Resolution { get; set; }
    }

    public interface IPayload<T>
    {
        T Payload { get; set; }
    }

    public interface IQueryResult<T> : ICommandResult, IPayload<T> { }
}
