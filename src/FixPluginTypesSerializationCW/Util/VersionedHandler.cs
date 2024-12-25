using System;

namespace FixPluginTypesSerializationCW.Util
{
    public struct VersionedHandler
    {
        public Version version;
        public object handler;

        public VersionedHandler(Version version, object handler)
        {
            this.version = version;
            this.handler = handler;
        }
    }
}
