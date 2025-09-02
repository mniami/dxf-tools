using DxfToolLib.Schemas;
using DxfToolLib.Schemas.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DxfToolLib.Helpers
{
    internal class SchemaFinder : ISchemaFinder
    {
        private readonly ISchemaStorage schemaStorage;
        private readonly IMatchesFinder matchesFinder;

        public SchemaFinder(ISchemaStorage schemaStorage, IMatchesFinder matchesFinder)
        {
            this.schemaStorage = schemaStorage;
            this.matchesFinder = matchesFinder;
        }
        public string[][] Matches(string schemaName, Dictionary<string, string>? variables, string[] input)
        {
            var schemaItems = schemaStorage.GetSchemaItemsByName(schemaName, variables);
            return matchesFinder.GetFoundMatches(schemaItems, input);
        }
    }
}
