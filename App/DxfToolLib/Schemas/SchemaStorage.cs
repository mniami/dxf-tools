using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Schemas
{
    public class SchemaStorage : ISchemaStorage
    {
        private readonly IDictionary<string, ISchema> schemas;

        public SchemaStorage(IEnumerable<ISchema> schemas) => this.schemas = schemas.ToDictionary(p => p.Name);

        public IList<string> GetSchemaItemsByName(string name, IDictionary<string, string> variables)
        {
            return schemas[name].GetSchemaItems(variables);
        }
    }
}
