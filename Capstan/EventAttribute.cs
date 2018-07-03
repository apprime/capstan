using System;

namespace Capstan
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventAttribute : Attribute
    {
        protected EventAttribute(string name, string category = "")
        {
            this.Name = name;
            this.Category = category;
        }

        public string Name { get; private set; }
        public string Category { get; private set; }
    }
}
