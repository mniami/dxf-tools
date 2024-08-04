
namespace DxfToolLib.Schemas
{
    public interface ISchemaStorage
    {
        IList<string> GetSchemaItemsByName(string name, IDictionary<string, string> variables);
    }
}