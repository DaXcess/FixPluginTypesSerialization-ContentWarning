using System;
using System.Runtime.InteropServices;
using FixPluginTypesSerialization.UnityPlayer.Structs.v2021.v1;
using MonoMod.RuntimeDetour;

namespace FixPluginTypesSerialization.Patchers
{
    internal unsafe class ConvertSeparatorsToPlatform
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void ConvertSeparatorsToPlatformDelegate(IntPtr assemblyStringPathName);

        private static NativeDetour _detourConvertSeparatorsToPlatform;
        private static ConvertSeparatorsToPlatformDelegate originalConvertSeparatorsToPlatform;

        internal static bool IsApplied { get; private set; }

        public unsafe void Apply(IntPtr from)
        {
            var hookPtr = Marshal.GetFunctionPointerForDelegate(new ConvertSeparatorsToPlatformDelegate(OnConvertSeparatorsToPlatformV1));
            _detourConvertSeparatorsToPlatform = new NativeDetour(from, hookPtr, new NativeDetourConfig { ManualApply = true });
            originalConvertSeparatorsToPlatform = _detourConvertSeparatorsToPlatform.GenerateTrampoline<ConvertSeparatorsToPlatformDelegate>();

            _detourConvertSeparatorsToPlatform.Apply();

            IsApplied = true;
        }

        internal static void Dispose()
        {
            if (_detourConvertSeparatorsToPlatform != null && _detourConvertSeparatorsToPlatform.IsApplied)
            {
                _detourConvertSeparatorsToPlatform.Dispose();
            }
            IsApplied = false;
        }

        private static unsafe void OnConvertSeparatorsToPlatformV1(IntPtr assemblyStringPathName)
        {
            var assemblyString = new AbsolutePathString(assemblyStringPathName);

            assemblyString.FixAbsolutePath();

            originalConvertSeparatorsToPlatform(assemblyStringPathName);
        }
    }
}