using System;
using System.Runtime.InteropServices;
using FixPluginTypesSerializationCW.UnityPlayer.Structs.v2021.v1;
using MonoMod.RuntimeDetour;

namespace FixPluginTypesSerializationCW.Patchers
{
    internal unsafe class AwakeFromLoad
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void AwakeFromLoadDelegate(IntPtr _monoManager, int awakeMode);

        private static AwakeFromLoadDelegate original;

        private static NativeDetour _detour;

        internal static MonoManager CurrentMonoManager;
        internal static bool IsApplied { get; private set; }

        public unsafe void Apply(IntPtr from)
        {
            var hookPtr =
                Marshal.GetFunctionPointerForDelegate(new AwakeFromLoadDelegate(OnAwakeFromLoad));

            _detour = new NativeDetour(from, hookPtr, new NativeDetourConfig { ManualApply = true });

            original = _detour.GenerateTrampoline<AwakeFromLoadDelegate>();
            _detour.Apply();

            IsApplied = true;
        }

        internal static void Dispose()
        {
            _detour?.Dispose();
            IsApplied = false;
        }

        private static unsafe void OnAwakeFromLoad(IntPtr _monoManager, int awakeMode)
        {
            CurrentMonoManager = new MonoManager();
            
            CurrentMonoManager.CopyNativeAssemblyListToManaged();

            IsAssemblyCreated.VanillaAssemblyCount = CurrentMonoManager.AssemblyCount;

            CurrentMonoManager.AddAssembliesToManagedList(Preload.PluginPaths);

            CurrentMonoManager.AllocNativeAssemblyListFromManaged();

            original(_monoManager, awakeMode);

            // Dispose detours as we don't need them anymore
            // and could hog resources for nothing otherwise
            ConvertSeparatorsToPlatform.Dispose();
            IsAssemblyCreated.Dispose();
        }
    }
}