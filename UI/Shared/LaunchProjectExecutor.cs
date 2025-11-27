using ApplicationCore.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Shared
{
    public static class LaunchProjectExecutor
    {
        public static void OpenIDE(ProcessStartInfo processInfo)
        {
            using Process process = new();
            process.StartInfo = processInfo;
            process.Start();
        }

        public static (bool, string) OpenIDEWithFileName(string fullFilePath, string devAppPath)
        {
            if (File.Exists(fullFilePath) is false)
            {
                return (false, $"File not found:\n{fullFilePath}");
            }

            OpenIDE(
                new()
                {
                    FileName = devAppPath,
                    Arguments = $"\"{fullFilePath}\"",
                    UseShellExecute = true,
                }
            );

            return (true, string.Empty);
        }
    }
}
