using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tortoise
{
    class SideProcess
    {
        private Process process;
        private ProcessStartInfo startInfo;
        private string name;
        private Action<SideProcess> callback;

        public SideProcess(string processName)
        {
            name = processName;
        }

        public void Start(string commandLine, Action<SideProcess> onCompleteCallback)
        {
            callback = onCompleteCallback;
            CreateStartInfo(commandLine);
            CreateProcess();
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }

        public string GetName()
        {
            return name;
        }

        public int GetResult()
        {
            return process.ExitCode;
        }

        private void CreateStartInfo(string commandLine)
        {
            startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C \"{commandLine}\"";

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
        }

        private void CreateProcess()
        {
            process = new Process();
            process.StartInfo = startInfo;

            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(process_Exited);
            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
        }

        private void process_Exited(object sender, System.EventArgs e)
        {
            callback(this);
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (!String.IsNullOrEmpty(dataReceivedEventArgs.Data))
            {
                Logger.WriteLine($"OutputDataReceived: {dataReceivedEventArgs.Data.ToString()}");
            }
        }

        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (!String.IsNullOrEmpty(dataReceivedEventArgs.Data))
            {
                Logger.WriteLine($"ErrorDataReceived: {dataReceivedEventArgs.Data.ToString()}");
            }
        }
    }
}
