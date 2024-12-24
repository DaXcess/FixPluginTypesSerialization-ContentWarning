using FixPluginTypesSerialization.UnityPlayer.Structs.Default;
using FixPluginTypesSerialization.Util;
using System;
using System.Collections.Generic;

namespace FixPluginTypesSerialization.UnityPlayer.Structs.v2021.v1
{
    public class MonoManager
    {
        public IntPtr Pointer { get => CommonUnityFunctions.ScriptingAssemblies; set { } }

        private unsafe RuntimeStatic<ScriptingAssemblies>* _this => (RuntimeStatic<ScriptingAssemblies>*)Pointer;

        private ScriptingAssemblies _originalScriptingAssemblies;

        public List<StringStorageDefaultV2> ManagedAssemblyList = new();
        public int AssemblyCount => ManagedAssemblyList.Count;

        public unsafe void CopyNativeAssemblyListToManaged()
        {
            MonoManagerCommon.CopyNativeAssemblyListToManagedV3(ManagedAssemblyList, _this->value->names);
        }

        public void AddAssembliesToManagedList(List<string> pluginAssemblyPaths)
        {
            MonoManagerCommon.AddAssembliesToManagedListV3(ManagedAssemblyList, pluginAssemblyPaths);
        }

        public unsafe void AllocNativeAssemblyListFromManaged()
        {
            MonoManagerCommon.AllocNativeAssemblyListFromManagedV3(ManagedAssemblyList, &_this->value->names);
        }

        public unsafe void PrintAssemblies()
        {
            MonoManagerCommon.PrintAssembliesV3(_this->value->names);
        }

        public unsafe void RestoreOriginalAssemblyNamesArrayPtr()
        {
            *_this->value = _originalScriptingAssemblies;
        }
    }
}
