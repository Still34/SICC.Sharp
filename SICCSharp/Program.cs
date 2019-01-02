using System;
using System.IO;
using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using SICCSharp.Extensions;

namespace SICCSharp
{
    /// <summary>
    ///     Written for NPTU System Software finals project.
    ///     This project is not meant for any serious SIC/XE compilation, as you can probably tell by the quality of the source
    ///     code. Use it to validate your homework at best, and even then, take its result with a grain of salt.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<LaunchOptions>(args).WithParsed(options =>
            {
                try
                {
                    // init logger
                    Console.Clear();
                    Log.Logger = CreateLoggerOnConfig(options.IsVerbose);

                    // basic check against ASM (no complex header check!)
                    if (!File.Exists(options.InputPath) && !Path.GetExtension(options.InputPath).EndsWith("asm"))
                    {
                        Log.Error("{InputPath} is invalid. Did you supply a valid file that has an extension of *.ASM?",
                            options.InputPath);
                        Environment.Exit(3);
                    }

                    // fallback directory
                    if (string.IsNullOrEmpty(options.OutputPath) || !Directory.Exists(options.OutputPath))
                        options.OutputPath = Directory.GetParent(options.InputPath).FullName;

                    Log.Information("Begin compilation for {inputPath}.", options.InputPath);
                    CompileHelper.CompileAsync(options.InputPath, options.OutputPath).GetAwaiter().GetResult();
                    Log.Information("Finished compilation...");
                }
                catch (Exception e)
                {
                    Log.Fatal(e, "An exception occurred during compilation.");
                }
            });
        }

        private static Logger CreateLoggerOnConfig(bool verbose)
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] [{Method}]{NewLine}{Message}{NewLine}{Exception}{NewLine}");

            // enable verbose logging
            if (verbose)
                loggerConfig.MinimumLevel.Verbose();

            return loggerConfig.CreateLogger();
        }
    }
}