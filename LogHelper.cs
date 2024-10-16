using Dotnet.Script.DependencyModel.Logging;
using Markdig.Extensions.Xmdl.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Markdig.Extensions.Xmdl.CSharp;

public static class LogHelper
{
    public static LogFactory CreateLogFactory(int verbosity)
    {
        var logLevel = (LogLevel)verbosity;
        return CreateLogFactory(logLevel);
    }

    public static LogFactory CreateLogFactory(string verbosity)
    {
        var logLevel = (LogLevel)LevelMapper.FromString(verbosity);
        return CreateLogFactory(logLevel);
    }

    public static LogFactory CreateLogFactory(LogLevel logLevel)
    {
        var loggerFilterOptions = new LoggerFilterOptions() { MinLevel = logLevel };

        var consoleLoggerProvider = new ConsoleLoggerProvider(new ConsoleOptionsMonitor());

        var loggerFactory = new LoggerFactory(new[] { consoleLoggerProvider }, loggerFilterOptions);

        return type =>
        {
            var logger = loggerFactory.CreateLogger(type);
            return (level, message, exception) =>
            {
                logger.Log((LogLevel)level, message, exception);
            };
        };
    }
}
