using System;

namespace Capstan
{
    /// <summary>
    /// Create an attribute that decorates classes that will be registered
    /// by capstan as events in runtime.
    /// Your events should inherit from Query, Command or Event base classes,
    /// in the Capstan namespace.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventAttribute : Attribute
    {
        /// <summary>
        /// This CTOR is called internally at startup of the framework.
        /// You do not need to call this directly.
        /// </summary>
        /// <param name="name">The name of the event</param>
        /// <param name="category">The category of events, defaults to "general"</param>
        protected EventAttribute(string name, string category = "general")
        {
            this.Name = name;
            this.Category = category;
        }

        public string Name { get; private set; }
        public string Category { get; private set; }
    }
}
