using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace UsbKey
{
    class Program
    {
        const string KeyFileName = "usb.key";
        static string MyKeyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, KeyFileName);

        static void Main(string[] args)
        {
            WinAPI.FreeConsole();

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            SystemEvents_SessionSwitch(null, new SessionSwitchEventArgs(SessionSwitchReason.SessionUnlock)); //холодный старт (первый вход в систему)

            new ManualResetEvent(false).WaitOne();
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason != SessionSwitchReason.SessionUnlock)
                return;

            var drives = DriveInfo.GetDrives().Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);

            var files = drives.SelectMany(x => x.RootDirectory.EnumerateFiles()).Where(x => x.Name == KeyFileName);

            if (!files.Any())
            {
                WinAPI.LockWorkStation();
                return;
            }

            bool login = false;

            var myKey = File.ReadAllBytes(MyKeyFile);
            foreach (var item in files)
            {
                var fileKey = File.ReadAllBytes(item.FullName);

                if (myKey.SequenceEqual(fileKey))
                {
                    login = true;
                    break;
                }
            }

            if (!login)
            {
                WinAPI.LockWorkStation();
            }
        }
    }
}
