using System;
using System.Runtime.InteropServices;

namespace FixPluginTypesSerialization.Util
{
    internal class CommonUnityFunctions
    {
        private enum AllocateOptions
        {
            None = 0,
            NullIfOutOfMemory
        };

        private delegate IntPtr MallocInternalFunc(ulong size, ulong allign, int label, AllocateOptions allocateOptions, IntPtr file, int line);
        private static MallocInternalFunc mallocInternal;

        private delegate void FreeAllocInternalFunc(IntPtr ptr, int label, IntPtr file, int line);
        private static FreeAllocInternalFunc _freeAllocInternal;

        public static IntPtr ScriptingAssemblies { get; private set; }

        public static void Init(IntPtr unityModule)
        {
            mallocInternal = (MallocInternalFunc)Marshal.GetDelegateForFunctionPointer(
                (IntPtr)(unityModule.ToInt64() + Preload.MallocInternalOffset), typeof(MallocInternalFunc));
            _freeAllocInternal = (FreeAllocInternalFunc)Marshal.GetDelegateForFunctionPointer(
                (IntPtr)(unityModule.ToInt64() + Preload.FreeAllocInternalOffset), typeof(FreeAllocInternalFunc));
            ScriptingAssemblies = (IntPtr)(unityModule.ToInt64() + Preload.ScriptingAssembliesOffset);
        }

        public static unsafe IntPtr MallocString(string str, int label, out ulong length)
        {
            //I couldn't for the life of me find how to adequately convert c# string to ANSI and fill existing pointer
            //that we would get from mallocInternal from c# so we're doing it this way
            var strPtr = Marshal.StringToHGlobalAnsi(str);

            length = (ulong)str.Length;
            //Ansi string might be longer than managed
            for (var c = (byte*)strPtr + length; *c != 0; c++, length++) { }

            var allocPtr = mallocInternal(length + 1, 0x10, label, AllocateOptions.NullIfOutOfMemory, IntPtr.Zero, 0);

            for (var i = 0ul; i <= length; i++)
            {
                ((byte*)allocPtr)[i] = ((byte*)strPtr)[i];
            }

            Marshal.FreeHGlobal(strPtr);

            return allocPtr;
        }

        public static void FreeAllocInternal(IntPtr ptr, int label)
        {
            _freeAllocInternal(ptr, label, IntPtr.Zero, 0);
        }
    }
}
