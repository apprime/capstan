using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace ExampleAllFeatures
{
    internal class ChattyEventFactory
    {
        public ChattyEvent Create(StringMessage input, IUnityContainer container)
        {
            return new ChattyEvent(input, container.Resolve<IEnterpriseBusinessDependency>());
        }
    }
}
