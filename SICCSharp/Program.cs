using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SICCSharp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }
    }

    public static class CompileHelper
    {
        public static async Task CompileAsync(string input, string output)
        {
            using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, 1024,
                FileOptions.Asynchronous))
            using (var inputFile = new StreamReader(inputStream))
            using (var outputStream = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                1024, FileOptions.Asynchronous))
            using (var outputFile = new StreamWriter(outputStream))
            {
            }
        }
    }
}