
namespace DxfToolLib.Helpers
{
    internal interface ISchemaFinder
    {
        string[] Matches(string schemaName, Dictionary<string, string>? variables, string[] input);
    }
}