using System;
using System.Diagnostics;
using System.Text;

namespace UnitTests.Helper
{
    public class RamDrive : IDisposable
    {
        public bool Ready { get; private set; }
        public StringBuilder Error = new StringBuilder();
        public StringBuilder Output = new StringBuilder();

        public RamDrive()
        {
            ExecRamdriveBatchfile("MOUNT");
        }

        public void Dispose()
        {
            ExecRamdriveBatchfile("UNMOUNT");
            Ready = false;
        }

        private void ExecRamdriveBatchfile(string action)
        {
            Ready = true;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = @"..\..\..\..\memdrive\setDrive.cmd",
                    Arguments = action,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            proc.OutputDataReceived += (sender, args) => Output.AppendLine(args.Data);
            proc.ErrorDataReceived += (sender, args) =>
            {
                if(args.Data == null) return;
                Error.AppendLine(args.Data);
                Ready = false;
            };

            if (!proc.Start()) throw new Exception("Error Mounting Ramdrive");
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
            proc.WaitForExit(10000);
            
        }
    }
}
