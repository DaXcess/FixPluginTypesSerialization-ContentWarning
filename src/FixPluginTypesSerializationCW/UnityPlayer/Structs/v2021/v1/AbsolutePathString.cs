﻿using FixPluginTypesSerializationCW.Patchers;
using FixPluginTypesSerializationCW.UnityPlayer.Structs.Default;
using FixPluginTypesSerializationCW.Util;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FixPluginTypesSerializationCW.UnityPlayer.Structs.v2021.v1
{
    public class AbsolutePathString(IntPtr pointer)
    {
        public IntPtr Pointer => pointer;

        private unsafe StringStorageDefaultV2* _this => (StringStorageDefaultV2*)Pointer;

        public unsafe void FixAbsolutePath()
        {
            if (_this->data_repr != StringRepresentation.Embedded && (_this->union.heap.data == 0 || _this->union.heap.size == 0))
            {
                return;
            }

            var pathNameStr = _this->data_repr switch
            {
                StringRepresentation.Embedded => Marshal.PtrToStringAnsi((IntPtr)_this->union.embedded.data),
                _ => Marshal.PtrToStringAnsi(_this->union.heap.data, (int)_this->union.heap.size),
            };

            var fileNameStr = Path.GetFileName(pathNameStr);
            var newPathIndex = Preload.PluginNames.IndexOf(fileNameStr);
            if (newPathIndex == -1)
            {
                return;
            }

            var newPath = Preload.PluginPaths[newPathIndex];
            var newNativePath = CommonUnityFunctions.MallocString(newPath, Preload.LabelMemStringId, out var length);
            if (_this->data_repr != StringRepresentation.Embedded)
            {
                CommonUnityFunctions.FreeAllocInternal(_this->union.heap.data, _this->label);
            }
            var str = _this;
            str->union = new StringStorageDefaultV2Union
            {
                heap = new HeapAllocatedRepresentationV2
                {
                    data = newNativePath,
                    capacity = length,
                    size = length
                }
            };
            str->data_repr = StringRepresentation.Heap;
            str->label = Preload.LabelMemStringId;
        }

        public unsafe string ToStringAnsi()
        {
            if (_this->data_repr != StringRepresentation.Embedded && (_this->union.heap.data == 0 || _this->union.heap.size == 0))
            {
                return null;
            }

            return _this->data_repr switch
            {
                StringRepresentation.Embedded => Marshal.PtrToStringAnsi((IntPtr)_this->union.embedded.data),
                _ => Marshal.PtrToStringAnsi(_this->union.heap.data, (int)_this->union.heap.size),
            };
        }
    }
}
