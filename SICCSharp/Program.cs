using System;
using System.IO;
using CommandLine;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SICCSharp.Extensions;

namespace SICCSharp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<LaunchOptions>(args).WithParsed( options =>
            {
                try
                {
                    var loggerConfig = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(theme: AnsiConsoleTheme.Code,
                            outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] [{Method}] {Message}{NewLine}");
                    if (options.IsVerbose)
                        loggerConfig.MinimumLevel.Verbose();
                    Log.Logger = loggerConfig.CreateLogger();
                    if (!File.Exists(options.InputPath) && !Path.GetExtension(options.InputPath).EndsWith("asm"))
                    {
                        Log.Error("{InputPath} is invalid. Did you supply a valid file that has an extension of *.ASM?",
                            options.InputPath);
                        Environment.Exit(3);
                    }

                    if (string.IsNullOrEmpty(options.OutputPath) || !Directory.Exists(options.OutputPath))
                        options.OutputPath = Directory.GetParent(options.InputPath).FullName;

                    CompileHelper.CompileAsync(options.InputPath, options.OutputPath).GetAwaiter().GetResult();
                    Log.Information("Finished compilation... Press any key to exit.");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Log.Fatal(e, "An exception occurred during compilation.");
                    Console.ReadKey();
                }
            });
        }
    }
}