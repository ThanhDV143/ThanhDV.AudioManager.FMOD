using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public static class PackageImporter
    {
        private const string DefaultBusesAssetFolder = "Assets/Packages/AudioManager/FMOD/SO";
        private const string DefaultBusesAssetPath = DefaultBusesAssetFolder + "/" + Common.BUS_SO_NAME + ".asset";

        static PackageImporter()
        {
            string packageVersion = GetPackageVersion();
            string editorPrefsKey = $"ThanhDV.AudioManager.FMOD.Version.{packageVersion}.Initialized";

            if (!EditorPrefs.HasKey(editorPrefsKey)) EditorPrefs.SetBool(editorPrefsKey, false);
            if (!EditorPrefs.GetBool(editorPrefsKey, false))
            {
                MakeAddressable();
                EditorPrefs.SetBool(editorPrefsKey, true);
            }
        }

        public static string GetPackageVersion()
        {
            string[] guids = AssetDatabase.FindAssets("t:Script PackageImporter");
            if (guids.Length == 0)
            {
                Debug.Log("<color=yellow>[AudioManager - FMOD] Could not find PackageImporter script to determine version!!!</color>");
                return "Undefined version";
            }
            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);

            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scriptPath);

            if (packageInfo == null) return "Undefined version";

            return packageInfo.version;
        }

        private static string FindSaveSettingsPath()
        {
            // Prefer a stable, deterministic location.
            if (AssetDatabase.LoadAssetAtPath<FMODBuses>(DefaultBusesAssetPath) != null)
                return DefaultBusesAssetPath;

            // Find by exact type (FMODBuses). Optionally filter by name via BUS_SO_NAME.
            string[] guids = AssetDatabase.FindAssets($"{Common.BUS_SO_NAME} t:{nameof(FMODBuses)}");
            if (guids == null || guids.Length == 0)
                guids = AssetDatabase.FindAssets($"t:{nameof(FMODBuses)}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.LoadAssetAtPath<FMODBuses>(path) != null)
                    return path;
            }

            return CreateSaveSettingsIfNotExist();
        }

        public static void MakeAddressable()
        {
            string assetName = $"{Common.BUS_SO_NAME}.asset";
            string assetPath = FindSaveSettingsPath();
            string guid = AssetDatabase.AssetPathToGUID(assetPath);

            if (string.IsNullOrEmpty(guid))
            {
                Debug.Log($"<color=red>[GameSaver] Could not find SaveSettings at path {assetPath} to make it addressable. GUID is null or empty!!!</color>");
                return;
            }

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                try
                {
                    AddressableAssetSettingsDefaultObject.GetSettings(true);
                    settings = AddressableAssetSettingsDefaultObject.Settings;
                }
                catch
                {
                    Debug.Log($"<color=red>[GameSaver] Addressable Asset Settings not found. Please initialize Addressables in your project (Window > Asset Management > Addressables > Groups, then click 'Create Addressables Settings')!!!</color>");
                    return;
                }
            }

            if (settings == null)
            {
                Debug.Log($"<color=red>[GameSaver] Failed to initialize Addressable Asset Settings!!!</color>");
                return;
            }

            AddressableAssetGroup group = settings.FindGroup("GameSaver");
            if (group == null)
            {
                try
                {
                    group = settings.CreateGroup("GameSaver", false, false, true, null, typeof(AddressableGroupSchemas.BundledAssetGroupSchema));
                    if (group == null)
                    {
                        Debug.Log($"<color=red>[GameSaver] Failed to create Addressable Asset Group 'GameSaver'!!!</color>");
                        return;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log($"<color=red>[GameSaver] Exception creating Addressable group: {ex.Message}. Please manually initialize Addressables first!!!</color>");
                    return;
                }
            }

            if (group.GetSchema<AddressableGroupSchemas.BundledAssetGroupSchema>() == null)
            {
                group.AddSchema<AddressableGroupSchemas.BundledAssetGroupSchema>();
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupSchemaAdded, group, true);
            }

            try
            {
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
                if (entry != null)
                {
                    entry.address = Constant.SAVE_SETTINGS_NAME;
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
                    Debug.Log($"<color=green>[GameSaver] Made ScriptableObject '{assetName}' addressable with address '{entry.address}' in group '{group.Name}'!!!</color>");
                }
                else
                {
                    Debug.Log($"<color=red>[GameSaver] Failed to create or move Addressable entry for {assetName} (GUID: {guid})!!!</color>");
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log($"<color=red>[GameSaver] Exception creating Addressable entry: {ex.Message}!!!</color>");
            }
        }
    }
}
