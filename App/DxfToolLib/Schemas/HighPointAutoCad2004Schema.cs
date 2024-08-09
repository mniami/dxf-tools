﻿using DxfToolLib.Schemas.Core;
using DxfToolLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Schemas
{
    public class HighPointAutoCad2004Schema: ISchema
    {
        public string Name
        {
            get
            {
                return KnownSchemas.HighPointAutoCad2004.NAME;
            }
        }

        public IList<string> GetSchemaItems(IDictionary<string, string>? variables)
        {
            var title = "";
            variables?.TryGetValue(KnownSchemas.HighPointAutoCad2000.FIELDS.TITLE, out title);

            return [
                "AcDbEntity",
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                title + ".*",
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                "Continuous",
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                "AcDbLine",
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
                Numbers.FLOATING_NUMBER_REGEX_PATTERN,
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
                ".*",
                $"({Numbers.FLOATING_NUMBER_REGEX_PATTERN})",
            ];
        }
    }
}