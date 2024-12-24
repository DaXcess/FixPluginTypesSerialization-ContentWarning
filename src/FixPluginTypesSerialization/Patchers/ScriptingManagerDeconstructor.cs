using System;
using System.Runtime.InteropServices;
using MonoMod.RuntimeDetour;

namespace FixPluginTypesSerialization.Patchers
{
    internal unsafe class ScriptingManagerDeconstructor
    {
        [UnmanagedFunctionPointer(CallingConvention.FastCall)]
        private delegate void ScriptingManagerDeconstructorDelegate(IntPtr scriptingManagerPtr);

        private static NativeDetour _detour;
        private static ScriptingManagerDeconstructorDelegate orig;

        internal static bool IsApplied { get; private set; }

        public unsafe void Apply(IntPtr from)
        {
            ApplyDetour(from);
            IsApplied = true;
        }

        private void ApplyDetour(IntPtr from)
        {
            var hookPtr = Marshal.GetFunctionPointerForDelegate(new ScriptingManagerDeconstructorDelegate(OnDeconstructor));
            _detour = new NativeDetour(from, hookPtr, new NativeDetourConfig { ManualApply = true });

            orig = _detour.GenerateTrampoline<ScriptingManagerDeconstructorDelegate>();
            _detour.Apply();
        }

        internal static void Dispose()
        {
            DisposeDetours();
            IsApplied = false;
        }

        private static void DisposeDetours()
        {
            if (_detour != null && _detour.IsApplied)
            {
                _detour.Dispose();
            }
        }

        private static unsafe void OnDeconstructor(IntPtr scriptingManagerPtr)
        {
            AwakeFromLoad.CurrentMonoManager.RestoreOriginalAssemblyNamesArrayPtr();
            Log.Info("Restored original AssemblyNames list");

            orig(scriptingManagerPtr);
        }
    }
}
