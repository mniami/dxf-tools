using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Schemas
{
    public interface ISchema
    {
        string Name { get; }
        IList<string> GetSchemaItems(IDictionary<string, string> variables);
    }
}
