using Markdig.Extensions.Xmdl;
using Markdig.Extensions.Xmdl.CSharp;
using System;

namespace Markdig.Extensions.Xmdl;

public static class XmdlCSharpExtensions
{
    public static MarkdownPipelineBuilder UseXmdlCSharp(this MarkdownPipelineBuilder pipeline, ExecutableCodeOptions options, Uri currentUri)
    {
        pipeline = pipeline.UseSimpleXmdl(currentUri);
        pipeline.Extensions.Add(new XmdlCSharpExtension(options, currentUri));
        return pipeline;
    }
}
