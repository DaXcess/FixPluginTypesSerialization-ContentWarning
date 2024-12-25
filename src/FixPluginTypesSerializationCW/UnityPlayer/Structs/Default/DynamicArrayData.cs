using System.Runtime.InteropServices;

namespace FixPluginTypesSerializationCW.UnityPlayer.Structs.Default
{
    // struct dynamic_array_detail::dynamic_array_data
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DynamicArrayData
    {
        public nint ptr;
        public int label;
        public ulong size;
        public ulong capacity;
    }
}
