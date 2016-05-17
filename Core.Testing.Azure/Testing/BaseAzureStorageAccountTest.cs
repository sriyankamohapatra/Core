using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sfa.Core.Testing
{
    public class BaseAzureStorageAccountTest : BaseTest
    {
        #region Constants

        protected readonly string StorageEmulatorDirectory = "C:\\Program Files (x86)\\Microsoft SDKs\\Azure\\Storage Emulator";
        protected readonly string StorageEmulatorExeName = "AzureStorageEmulator.exe";
        protected readonly string StorageEmulatorProcessName = "AzureStorageEmulator";
        protected readonly string StorageEmulatorExeStartCommand = "start";
        protected readonly string StorageEmulatorExeStopCommand = "stop";
        protected readonly string StorageEmulatorExeClearDataCommand = "clear all";

        #endregion


        #region Life Cycle

        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();
            StartEmulator();
            ClearEmulatorData();
        }

        protected override void TearDownEachTest()
        {
            base.TearDownEachTest();
            if (!KeepAzureEmulatorDataAtEndOfTest)
            {
                ClearEmulatorData();
            }
        }

        protected bool KeepAzureEmulatorDataAtEndOfTest { get; set; }

        #endregion


        #region Emulator Helpers


        protected void StartEmulator()
        {
            CallEmulatorProcess(StorageEmulatorExeStartCommand);
        }

        protected void ClearEmulatorData()
        {
            CallEmulatorProcess(StorageEmulatorExeClearDataCommand);

        }

        private void CallEmulatorProcess(string args)
        {
            var path = Path.Combine(StorageEmulatorDirectory, StorageEmulatorExeName);
            var process = Process.Start(path, args);
            process.WaitForExit();
        }

        protected void StopEmulator()
        {
            CallEmulatorProcess(StorageEmulatorExeStopCommand);
            EmulatorProcess?.Kill();
        }

        protected Process EmulatorProcess => Process.GetProcessesByName(StorageEmulatorProcessName).FirstOrDefault();

        protected bool IsEmulatorRunning => EmulatorProcess != null;

        #endregion
    }
}