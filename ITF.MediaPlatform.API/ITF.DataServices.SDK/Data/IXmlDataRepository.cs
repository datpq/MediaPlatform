namespace ITF.DataServices.SDK.Data
{
    public interface IXmlDataRepository : IDataRepository
    {
        T Deserialize<T>(string xmlFilePath) where T : class;
    }
}
