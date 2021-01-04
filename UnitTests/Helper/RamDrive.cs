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
            var pInfo = new ProcessStartInfo(@"..\..\..\..\memdrive\setDrive.cmd", action);
            var proc = new Process { StartInfo = pInfo, EnableRaisingEvents = true };

            if (!proc.Start()) throw new Exception("Error Mounting Ramdrive");
            proc.WaitForExit(10000);
            proc.OutputDataReceived += (sender, args) => Output.AppendLine(args.Data);
            proc.ErrorDataReceived += (sender, args) =>
            {
                Error.AppendLine(args.Data);
                Ready = false;
            };
        }
    }
}
