
namespace DxfToolLib.Helpers
{
    internal interface IMatchesFinder
    {
        string[][] GetFoundMatches(IList<string> items, string[] input);
    }
}