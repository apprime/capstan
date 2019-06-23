using System;

namespace ExampleAllFeatures
{
    /// <summary>
    /// This is not used in the example, it only exists to demonstrate dependency resolution.
    /// </summary>
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
