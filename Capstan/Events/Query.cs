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
