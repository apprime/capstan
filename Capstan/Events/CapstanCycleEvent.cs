using Capstan.Core;
using System.Collections.Generic;

namespace Capstan.Events
{
    /// <summary>
    /// This is an internal event used by Capstan.
    /// It cycles through all Activists to see if any should
    /// be activated.
    /// </summary>
    internal static class CapstanCycleEvent
    {
        private static List<Core.Activist> activists;

        internal static void RegisterActivist(Core.Activist activist)
        {
            activists.Add(activist);
        }

        public static void OnTimerEvent(object sender)
        {
            foreach (var activist in activists.Where(i => i.Condition()))
            {
                activist.Activate();
            }
        }
    }
}
