// -----------------------------------------------------------------------
//  <copyright file="Convert.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;

namespace Valkyrie.UniversalPrint.Formats
{
    public static class Convert
    {
        public static void OpenXpsToXps(FileInfo input, FileInfo output)
        {
            RunXpsConverter("/XPS", input, output);
        }

        private static void RunXpsConverter(string flag, FileInfo input, FileInfo output)
        {
            var processStartInfo = new ProcessStartInfo("Microsoft\\XpsConverter.exe", $"{flag} \"/InputFile={input.FullName}\" \"/OutputFile={output.FullName}\"")
            {
                RedirectStandardError = true, 
                RedirectStandardOutput = true, 
                UseShellExecute = false
            };

            var process = Process.Start(processStartInfo);

            process?.StandardOutput.ReadToEnd();

            process?.WaitForExit();

            if (process == null || process.ExitCode != 0)
                throw new InvalidOperationException("Failed to convert OXPS to XPS.");
        }

        public static void XpsToOpenXps(FileInfo input, FileInfo output)
        {
            RunXpsConverter("/OpenXPS", input, output);
        }
    }
}