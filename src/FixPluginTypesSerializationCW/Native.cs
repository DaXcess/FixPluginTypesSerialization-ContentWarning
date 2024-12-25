using System;
using System.Runtime.InteropServices;

namespace FixPluginTypesSerializationCW
{
    public static class Native
    {
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);
    }
}