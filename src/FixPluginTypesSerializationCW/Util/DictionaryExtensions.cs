using System;

namespace FixPluginTypesSerializationCW.Util
{
    internal static class DictionaryExtensions
    {
        public static void Deconstruct(this VersionedHandler versionedHandler, out Version version, out object handler)
        {
            version = versionedHandler.version;
            handler = versionedHandler.handler;
        }
    }
}
