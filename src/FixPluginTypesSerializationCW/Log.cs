using System;
using System.Runtime.InteropServices;

namespace FixPluginTypesSerializationCW
{
    internal static class Log
    {
        private enum UnityLogType
        {
            kUnityLogTypeError,
            kUnityLogTypeWarning,
            kUnityLogTypeLog
        }

        private delegate void LogDelegate(UnityLogType type, string message, string filename, int fileLine);

        private static LogDelegate nativeLog;

        internal static unsafe void Init()
        {
            var modBase = Native.GetModuleHandle("preloader");
            var ifacePtr = (IntPtr**)((IntPtr)(modBase.ToInt64() + Preload.UnityLogPointer)).ToPointer();
            var loggerPtr = **ifacePtr;

            nativeLog = Marshal.GetDelegateForFunctionPointer<LogDelegate>(loggerPtr);
        }

        internal static void Error(object data) => nativeLog(UnityLogType.kUnityLogTypeError,
            $"[FixPluginTypesSerializationCW] {data}",
            "FixPluginTypesSerializationCW", 1);

        internal static void Info(object data) =>
            nativeLog(UnityLogType.kUnityLogTypeLog, $"[FixPluginTypesSerializationCW] {data}",
                "FixPluginTypesSerializationCW", 1);

        internal static void Warning(object data) => nativeLog(UnityLogType.kUnityLogTypeWarning,
            $"[FixPluginTypesSerializationCW] {data}",
            "FixPluginTypesSerializationCW", 1);
    }
}