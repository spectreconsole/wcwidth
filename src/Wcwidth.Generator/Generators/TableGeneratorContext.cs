using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Spectre.IO;
using Wcwidth;

namespace Generator.Generators;

public sealed class TableGeneratorContext
{
    public required DirectoryPath DataPath { get; init; }

    public List<string> GetUnicodeVersions()
    {
        var result = new List<string>();
        foreach (var field in typeof(Unicode).GetFields().Where(x => x.IsStatic))
        {
            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            if (attr == null || string.IsNullOrWhiteSpace(attr.Description))
            {
                throw new InvalidOperationException("Unicode version enum is missing version attribute.");
            }

            result.Add(attr.Description);
        }

        return result;
    }
}