﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxfToolLib.Schemas.Core;

namespace DxfToolLib.Schemas
{
    internal class CodePageSchema : ISchema
    {
        public string Name
        {
            get
            {
                return KnownSchemas.CodePage.NAME;
            }
        }

        public IList<string> GetSchemaItems(IDictionary<string, string>? variables)
        {
            return [
                "\\$DWGCODEPAGE",
                ".*",
                "(.*)",
            ];
        }
    }
}
