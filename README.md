# FixPluginTypesSerialization - Content Warning

This fork of [FixPluginTypesSerialization](https://github.com/xiaoxiao921/FixPluginTypesSerialization) is a _very_ trimmed down version of the original.

All offsets are hardcoded directly into the assembly, and the preloader has been modified to work with Content Warning's built-in mod loader.

**This fork does not work with BepInEx**

## Original description

Hook into the native Unity engine for adding ~~BepInEx~~ plugin assemblies into the assembly list that is normally used for the assemblies sitting in the game Managed/ folder.

This solve a bug where custom Serializable structs and such stored in plugin assemblies are not properly getting deserialized by the engine.
