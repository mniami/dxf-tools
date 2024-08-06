namespace DxfToolLib.Schemas.Core
{
    public interface ISchemaStorage
    {
        IList<string> GetSchemaItemsByName(string name, IDictionary<string, string>? variables);
    }
}