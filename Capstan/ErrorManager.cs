using Capstan.Core;
using System;
using System.Collections.Generic;

namespace Capstan
{
    /// <summary>
    /// This is an abstract implementation of an Error Manager.
    /// Because Capstan does not follow a 1:1 mapping between 
    /// Request and Response, we have this instead.
    /// 
    /// When an error arises, use Capstan to return the message to sender.
    /// Only sender will be informed of the error, but the return method
    /// can be overridden to include for example a logger service, or 
    /// a logger client that also Receive()'s the error.
    /// </summary>
    public abstract class ErrorManager<TOutput>
    {
        public abstract TOutput ParseError(Exception ex);

        public virtual void ReturnToSender(int senderId, Exception ex)
        {
            if (Clients.ContainsKey(senderId))
            {
                Clients[senderId].Receive(ParseError(ex));
            }
            else
            {
                //TODO: Log error when no sender was found.
            }

        }

        protected abstract Dictionary<int, Receiver<TOutput>> Clients { get; set; }

        /// <summary>
        /// This is a standard error that should be returned when a route cannot be found.
        /// </summary>
        public virtual ArgumentException ArgumentException(string key)
        {
            return new ArgumentException(
                $"Incoming event with key {key} does not exist as either an synchronous or asynchronous route." +
                $" Change the input key, or add a route to the engine during config, using either of the ConfigRoute methods.");
        }
    }
}