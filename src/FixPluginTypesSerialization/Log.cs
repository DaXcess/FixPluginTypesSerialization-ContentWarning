using System;

namespace FixPluginTypesSerialization
{
    internal static class Log
    {
        internal static void Debug(object data) => Console.WriteLine(data);
        internal static void Error(object data) => Console.WriteLine(data);
        internal static void Fatal(object data) => Console.WriteLine(data);
        internal static void Info(object data) => Console.WriteLine(data);
        internal static void Message(object data) => Console.WriteLine(data);
        internal static void Warning(object data) => Console.WriteLine(data);
    }
}