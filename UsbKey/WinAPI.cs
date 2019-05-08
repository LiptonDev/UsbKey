using System.Runtime.InteropServices;

namespace UsbKey
{
    static class WinAPI
    {
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("user32")]
        public static extern void LockWorkStation();
    }
}
