using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FixPluginTypesSerialization.Patchers;
using FixPluginTypesSerialization.Util;
using Steamworks;

namespace FixPluginTypesSerialization
{
    public static class Preload
    {
        #region CW 1.19.e
        
        public const int LabelMemStringId = 0x49;
        public const long MonoManagerAwakeFromLoadOffset = 0x7819E0;
        public const long MonoManagerIsAssemblyCreatedOffset = 0x781800;
        public const long ScriptingManagerDeconstructorOffset = 0x796700;
        public const long ConvertSeparatorsToPlatformOffset = 0x68CA90;
        public const long FreeAllocInternalOffset = 0x656E20;
        public const long MallocInternalOffset = 0x359EA0;
        public const long ScriptingAssembliesOffset = 0x1C0CA40;
        public const long UnityLogPointer = 0x26e30;
        
        #endregion
        
        public static string GameRoot => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName);
        public static string LocalPluginsPath => Path.Combine(GameRoot, "Plugins");

        public static List<string> PluginPaths { get; private set; }
        public static List<string> PluginNames => PluginPaths.Select(p => Path.GetFileName(p)).ToList();

        public static bool IsNetAssembly(string fileName)
        {
            try
            {
                AssemblyName.GetAssemblyName(fileName);
            }
            catch (BadImageFormatException)
            {
                return false;
            }

            return true;
        }

        public static void PreloadInit()
        {
            Log.Init();
            
            try
            {
                InitializeInternal();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to initialize plugin types serialization fix: ({e.GetType()}) {e.Message}. Some plugins may not work properly.");
                Log.Error(e);
            }
        }

        private static void InitializeInternal()
        {
            PopulatePluginPaths();
            DetourUnityPlayer();
        }

        private static void PopulatePluginPaths()
        {
            Log.Info("Searching for plugins...");
            
            var result = new List<string>();

            // 1. Local Plugins
            result.AddRange(Directory.GetFiles(LocalPluginsPath, "*.dll", SearchOption.AllDirectories)
                .Where(IsNetAssembly));
                
            // 2. Subscribed Plugins
            
            // TODO: Remove this once preloader order has been fixed
            SteamAPI.Init();
            
            var count = SteamUGC.GetNumSubscribedItems();
            var publishedFiles = new PublishedFileId_t[count];
            SteamUGC.GetSubscribedItems(publishedFiles, count);

            foreach (var fileId in publishedFiles)
            {
                if (!SteamUGC.GetItemInstallInfo(fileId, out _, out var directory, 2048, out _))
                    continue;

                result.AddRange(Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories)
                    .Where(IsNetAssembly));
            }

            PluginPaths = result;
            
            Log.Info($"Found {PluginPaths.Count} path{(PluginPaths.Count == 1 ? "" : "s")}");
        }

        private static unsafe void DetourUnityPlayer()
        {
            static bool IsUnityPlayer(ProcessModule p)
            {
                return p.ModuleName.ToLowerInvariant().Contains("unityplayer");
            }

            var proc = Process.GetCurrentProcess().Modules
                .Cast<ProcessModule>()
                .FirstOrDefault(IsUnityPlayer) ?? Process.GetCurrentProcess().MainModule;

            CommonUnityFunctions.Init(proc.BaseAddress);

            var awakeFromLoadPatcher = new AwakeFromLoad();
            var isAssemblyCreatedPatcher = new IsAssemblyCreated();
            var scriptingManagerDeconstructorPatcher = new ScriptingManagerDeconstructor();
            var convertSeparatorsToPlatformPatcher = new ConvertSeparatorsToPlatform();

            awakeFromLoadPatcher.Apply((IntPtr)(proc.BaseAddress.ToInt64() + MonoManagerAwakeFromLoadOffset));
            isAssemblyCreatedPatcher.Apply((IntPtr)(proc.BaseAddress.ToInt64() + MonoManagerIsAssemblyCreatedOffset));
            convertSeparatorsToPlatformPatcher.Apply((IntPtr)(proc.BaseAddress.ToInt64() +
                                                              ConvertSeparatorsToPlatformOffset));
            scriptingManagerDeconstructorPatcher.Apply((IntPtr)(proc.BaseAddress.ToInt64() +
                                                                ScriptingManagerDeconstructorOffset));
        }
    }
}

// Make the mod show up in the mod list
[ContentWarningPlugin("FixPluginTypesSerialization", "1.0.0", true)]
internal class Plugin
{
}
