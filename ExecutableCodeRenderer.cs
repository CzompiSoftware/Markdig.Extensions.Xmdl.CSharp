using System;
using Microsoft.CodeAnalysis;
using System.IO;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Dotnet.Script.DependencyModel.Context;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Markdig.Extensions.Xmdl.ExecutableCode;

namespace Markdig.Extensions.Xmdl.CSharp;

internal class ExecutableCodeRenderer(IExecutableCodeOptions options, MarkdownPipeline pipeline) : Xmdl.ExecutableCode.ExecutableCodeRenderer(options, pipeline)
{
    private readonly IExecutableCodeOptions _options = options;
    private readonly MarkdownPipeline _pipeline = pipeline;

    public override async Task<(ExecutableCodeState, ExecutableCodeResult, List<string>)> ExecuteAsync(string script, MarkdownParserContext context, string previousScript = null)
    {
        List<string> errors = new();
        ScriptContext scriptContext = new(SourceText.From((previousScript ?? Globals.GlobalScript) ?? script), _options.WorkingDirectory ?? Directory.GetCurrentDirectory(), _options.Arguments, null, ((ExecutableCodeOptions)_options).OptimizationLevel ?? OptimizationLevel.Debug, ScriptMode.Eval);
        ScriptCompilationContext<object> compilationContext = Globals.ScriptCompiler.CreateCompilationContext<object, CommandLineScriptGlobals>(scriptContext);
        var globals = new CommandLineScriptGlobals(Globals.ScriptConsole.Out, CSharpObjectFormatter.Instance);
        ScriptState scriptState = null;
        try
        {
            scriptState = await compilationContext.Script.ContinueWith(script).RunAsync(globals);
        }
        catch (Exception ex)
        {
            errors.Add(Markdown.ToHtml(BuildMarkdownExceptionMessage("C#", ex, _options.IsDebugMode), _pipeline));
        }
        var result = new ExecutableCodeResult()
        {
            Errors = compilationContext.Errors.Select(e => new ExecutableCodeError(e.Severity.ToString(), "C#", e.ToString())).ToList(),
        };
        var state = new ExecutableCodeState()
        {
            Exception = scriptState.Exception,
            ReturnValue = scriptState.ReturnValue,
            //Script = scriptState.Script,
            //Variables = scriptState.Variables,
        };
        return (state, result, errors);
    }

}