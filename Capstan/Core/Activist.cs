using System.Threading.Tasks;

namespace Capstan.Core
{
    /// <summary>
    /// An activist is a class that is scheduled to do things.
    /// These things are not reaction to other things, 
    /// they are simply created by the system.
    /// </summary>
    public abstract class Activist<IncomingType, ReturnedType> where IncomingType : Message
    {
        internal Broadcaster<IncomingType, ReturnedType>  Broadcaster { set; private get; }
        public abstract bool Condition();
        public abstract Task Activate();
    }
}
