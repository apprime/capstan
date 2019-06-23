namespace ExampleAllFeatures
{
    public interface IEnterpriseBusinessDependency
    {
        void GenerateXml(string text);
        byte[] ToExcel(object xmlDocument);
    }
}
