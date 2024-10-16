using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdig.Extensions.Xmdl.CSharp;

internal class Globals
{
    private static List<string> _globalScriptReferences = ["Markdig.Extensions.Xmdl.dll"];
    private static List<string> _globalScriptUsings = ["Markdig.Extensions.Xmdl"];

    public static string PreviousScript { get; internal set; }
    public static string GlobalScript =>
        $"#r \"{string.Join("\"\r\n#r \"", _globalScriptReferences)}\"\r\n" +
        $"using {string.Join(";\r\nusing ", _globalScriptUsings)};\r\n";

    public static LogFactory LogFactory { get; internal set; }
    public static ScriptCompiler ScriptCompiler { get; internal set; }
    public static ScriptConsole ScriptConsole { get; internal set; }

    public static void AddToGlobalScript(string[] references = null, string[] usings = null)
    {
        if (references != null)
        {
            _globalScriptReferences.AddRange(references);
            _globalScriptReferences = _globalScriptReferences.Distinct().ToList();
        }

        if (usings != null)
        {
            _globalScriptUsings.AddRange(usings);
            _globalScriptUsings = _globalScriptUsings.Distinct().ToList();
        }
    }
}