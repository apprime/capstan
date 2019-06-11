namespace ExampleAllFeatures
{
    partial class Program
    {
        public interface IEnterpriseBusinessDependency
        {
            void GenerateXml(string text);
            byte[] ToExcel(object xmlDocument);
        }
    }
}
