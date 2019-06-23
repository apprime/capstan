using Capstan.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capstan.Events
{
    /// <summary>
    /// This is an internal event used by Capstan.
    /// It cycles through all Activists to see if any should
    /// be activated.
    /// </summary>
    internal static class CapstanCycleEvent<IncomingType, ReturnedType> where IncomingType : Message
    {
        private static List<Activist<IncomingType, ReturnedType>> activists = new List<Activist<IncomingType, ReturnedType>>();
        internal static bool Cycling = false;

        internal static void RegisterActivist(Activist<IncomingType, ReturnedType> activist)
        {
            activists.Add(activist);
        }

        public static void OnTimerEvent(object sender)
        {
            if (!Cycling) { return; }

            //foreach(var a in activists)
            //{
            //    if(a.Condition())
            //    {
            //        a.Activate();
            //    }
            //}

            Task.Factory.StartNew
            (
                () => Parallel.ForEach(activists.Where(i => i.Condition()), i => i.Activate())
            );
        }
    }
}
