using Capstan.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstan.Events
{
    /// <summary>
    /// This is an internal event used by Capstan.
    /// It cycles through all Activists to see if any should
    /// be activated.
    /// </summary>
    internal static class CapstanCycleEvent
    {
        private static List<Activist> activists = new List<Activist>();

        internal static void RegisterActivist(Activist activist)
        {
            activists.Add(activist);
        }

        public static void OnTimerEvent(object sender)
        {
            Task.Factory.StartNew(() =>
                Parallel.ForEach(activists.Where(i => i.Condition()), i => i.Activate())
            );
        }
    }
}
