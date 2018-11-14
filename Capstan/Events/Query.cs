using System;
using System.Threading.Tasks;

namespace Capstan.Events
{
    public abstract class Query<T> : CapstanEvent
    {
        protected IQueryResult<T> Result;

        public async Task<IQueryResult<T>> Process()
        {
            await GatherData();
            await Resolve();
            return Result;
        }
    }
}
