using System;
using System.Runtime.InteropServices;
using MonoMod.RuntimeDetour;

namespace FixPluginTypesSerialization.Patchers
{
    internal unsafe class IsAssemblyCreated
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool IsAssemblyCreatedDelegate(IntPtr _monoManager, int index);

        private static IsAssemblyCreatedDelegate original;

        private static NativeDetour _detour;

        internal static bool IsApplied { get; private set; }

        internal static int VanillaAssemblyCount;

        public unsafe void Apply(IntPtr from)
        {
            var hookPtr =
                Marshal.GetFunctionPointerForDelegate(new IsAssemblyCreatedDelegate(OnIsAssemblyCreated));

            _detour = new NativeDetour(from, hookPtr, new NativeDetourConfig {ManualApply = true});

            original = _detour.GenerateTrampoline<IsAssemblyCreatedDelegate>();
            _detour?.Apply();

            IsApplied = true;
        }

        internal static void Dispose()
        {
            _detour?.Dispose();
            IsApplied = false;
        }

        private static unsafe bool OnIsAssemblyCreated(IntPtr _monoManager, int index)
        {
            if (index >= VanillaAssemblyCount)
            {
                return true;
            }

            return original(_monoManager, index);
        }
    }
}