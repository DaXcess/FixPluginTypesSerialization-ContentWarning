using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FixPluginTypesSerialization.UnityPlayer.Structs.Default;

namespace FixPluginTypesSerialization.Util
{
    internal static class MonoManagerCommon
    {
        public static unsafe void CopyNativeAssemblyListToManagedV3(List<StringStorageDefaultV2> managedAssemblyList, DynamicArrayData assemblyNames)
        {
            managedAssemblyList.Clear();

            ulong i = 0;
            for (StringStorageDefaultV2* s = (StringStorageDefaultV2*)assemblyNames.ptr;
                i < assemblyNames.size;
                s++, i++)
            {
                managedAssemblyList.Add(*s);
            }
        }

        public static unsafe void AddAssembliesToManagedListV3(List<StringStorageDefaultV2> managedAssemblyList, List<string> pluginAssemblyPaths)
        {
            foreach (var pluginAssemblyPath in pluginAssemblyPaths)
            {
                var pluginAssemblyName = Path.GetFileName(pluginAssemblyPath);
                var length = (ulong)pluginAssemblyName.Length;
                var strPtr = Marshal.StringToHGlobalAnsi(pluginAssemblyName);
                //Ansi string might be longer than managed
                for (var c = (byte*)strPtr + length; *c != 0; c++, length++) { }

                var assemblyString = new StringStorageDefaultV2
                {
                    union = new StringStorageDefaultV2Union
                    {
                        heap = new HeapAllocatedRepresentationV2
                        {
                            data = strPtr,
                            capacity = length,
                            size = length,
                        }
                    },
                    data_repr = StringRepresentation.Heap,
                    label = Preload.LabelMemStringId,
                };

                managedAssemblyList.Add(assemblyString);
            }
        }

        public static unsafe void AllocNativeAssemblyListFromManagedV3(List<StringStorageDefaultV2> managedAssemblyList, DynamicArrayData* assemblyNames)
        {
            var nativeArray = (StringStorageDefaultV2*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(StringStorageDefaultV2)) * managedAssemblyList.Count);

            var i = 0;
            for (StringStorageDefaultV2* s = nativeArray; i < managedAssemblyList.Count; s++, i++)
            {
                *s = managedAssemblyList[i];
            }

            assemblyNames->ptr = (nint)nativeArray;
            assemblyNames->size = (ulong)managedAssemblyList.Count;
            assemblyNames->capacity = assemblyNames->size;
        }

        public static unsafe void PrintAssembliesV3(DynamicArrayData assemblyNames)
        {
            ulong i = 0;
            for (StringStorageDefaultV2* s = (StringStorageDefaultV2*)assemblyNames.ptr;
                i < assemblyNames.size;
                s++, i++)
            {
                if (s->data_repr == StringRepresentation.Embedded)
                {
                    Log.Warning($"Ass: {Marshal.PtrToStringAnsi((IntPtr)s->union.embedded.data)} | label : {s->label:X}");
                }
                else
                {
                    Log.Warning($"Ass: {Marshal.PtrToStringAnsi(s->union.heap.data, (int)s->union.heap.size)} | label : {s->label:X}");
                }
            }
        }
    }
}
