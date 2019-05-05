using Capstan.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Capstan.Events
{
    internal static class CapstanCycleEvent
    {
        private static List<IActivist> activists;

        public static void RegisterActivist(IActivist activist)
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
