using System;
using System.Threading.Tasks;


/*
 Currrently removed due to following reasoning:
 A query returns information, but we already have an
 asynchronous way of returning information to a single user.
 These are called broadcasters.
 Using broadcasters inside of any event makes the event return data separately.

This solves a different problem for us, which is what happens if user A changes game state,
then we need to tell user B. If we can tell user B in a separate broadcast, why not also use
these for queries?

I.E:
We *never* respond to messages from clients. We only ever send messages back when we feel like it.
After all, any Query should be a Get[SomeInfo]() like GetTopGoats()
TopGoatsEvent should have a broadcast in it, possibly only broadcasting to the origin of the Event,
meaning a personal query -- but there is nothing that states it must behave like this.
 */
//namespace Capstan.Events
//{
//    public abstract class Query<T> : CapstanEvent
//    {
//        protected IQueryResult<T> Result;

//        public async Task<IQueryResult<T>> Process()
//        {
//            return Result;
//        }
//    }
//}
