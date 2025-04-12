using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Logging;
using Markdig.Extensions.Xmdl.ExecutableCode;
using Markdig.Renderers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Extensions.Xmdl.CSharp;

public class XmdlCSharpExtension : XmdlBaseExtension
{

    internal static Func<Microsoft.Extensions.Logging.LogLevel, bool, LogFactory> CreateLogFactory = (verbosity, debugMode) =>
    {
        return LogHelper.CreateLogFactory(debugMode ? (int)LogLevel.Trace : verbosity);
    };

    public XmdlCSharpExtension() : base() { }

    public XmdlCSharpExtension(ExecutableCodeOptions options, Uri currentUri = null) : base(options, currentUri)
    {
        Globals.LogFactory = CreateLogFactory(options.MinimumLogLevel, options.IsDebugMode);
        Globals.ScriptCompiler = new(Globals.LogFactory, !((ExecutableCodeOptions)_options).NoCache);
        Globals.ScriptConsole = ScriptConsole.Default;
    }

    public override void Setup(MarkdownPipelineBuilder pipeline)
    {
        base.Setup(pipeline);
        pipeline.InlineParsers.ReplaceOrAdd<ExecutableCodeInlineParser>(new ExecutableCodeInlineParser("cs", "C#"));
        pipeline.BlockParsers.ReplaceOrAdd<ExecutableCodeBlockParser>(new ExecutableCodeBlockParser("cs", "C#"));
    }
    public override void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        base.Setup(pipeline, renderer);
        var codeRenderer = new ExecutableCodeRenderer(_options, pipeline);
        renderer.ObjectRenderers.ReplaceOrAdd<ExecutableCodeInlineRenderer>(new ExecutableCodeInlineRenderer(codeRenderer, _options, pipeline));
        renderer.ObjectRenderers.ReplaceOrAdd<ExecutableCodeBlockRenderer>(new ExecutableCodeBlockRenderer(codeRenderer, _options, pipeline));
    }

}
