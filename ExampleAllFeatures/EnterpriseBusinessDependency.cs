using System;

namespace ExampleAllFeatures
{
    partial class Program
    {
        public class EnterpriseBusinessDependency : IEnterpriseBusinessDependency
        {
            public void GenerateXml(string text)
            {
                throw new NotImplementedException();
            }

            public byte[] ToExcel(object xmlDocument)
            {
                throw new NotImplementedException();
            }
        }
    }
}
