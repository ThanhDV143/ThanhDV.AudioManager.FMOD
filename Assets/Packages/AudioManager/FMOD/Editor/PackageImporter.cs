using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using AddressableGroupSchemas = UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace ThanhDV.AudioManager.FMOD
{
    public static class PackageImporter
    {
        static PackageImporter()
        {
            string packageVersion = GetPackageVersion();
            string editorPrefsKey = $"{Common.EDITOR_PREF_KEY_PREFIX}{packageVersion}";

            if (!EditorPrefs.HasKey(editorPrefsKey)) EditorPrefs.SetBool(editorPrefsKey, false);
            if (!EditorPrefs.GetBool(editorPrefsKey, false))
            {
                MakeAddressable();
                EditorPrefs.SetBool(editorPrefsKey, true);
            }
        }

        private static string FindSOPath()
        {
            return FMODReferencesEditorAsset.FindOrCreatePath();
        }

        public static string GetPackageVersion()
        {
            string[] guids = AssetDatabase.FindAssets("t:MonoScript PackageImporter");

            foreach (string guid in guids)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

                if (script != null && script.GetClass() == typeof(PackageImporter))
                {
                    var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scriptPath);
                    if (packageInfo != null) return packageInfo.version;
                }
            }

            DebugLog.Warning("Could not find PackageImporter script to determine version!!!");
            return "Undefined version";
        }

        public static void MakeAddressable()
        {
            string assetName = $"{Common.FMOD_REF_SO_NAME}{Common.ASSET_EXTENSION}";
            string assetPath = FindSOPath();
            string guid = AssetDatabase.AssetPathToGUID(assetPath);

            if (string.IsNullOrEmpty(guid))
            {
                DebugLog.Error($"Could not find SaveSettings at path {assetPath} to make it addressable. GUID is null or empty!!!");
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
                    DebugLog.Error("Addressable Asset Settings not found. Please initialize Addressables in your project (Window > Asset Management > Addressables > Groups, then click 'Create Addressables Settings')!!!");
                    return;
                }
            }

            if (settings == null)
            {
                DebugLog.Error("Failed to initialize Addressable Asset Settings!!!");
                return;
            }

            AddressableAssetGroup group = settings.FindGroup(Common.ADDRESSABLE_GROUP);
            if (group == null)
            {
                try
                {
                    group = settings.CreateGroup(Common.ADDRESSABLE_GROUP, false, false, true, null, typeof(AddressableGroupSchemas.BundledAssetGroupSchema));
                    if (group == null)
                    {
                        DebugLog.Error($"Failed to create Addressable Asset Group '{Common.ADDRESSABLE_GROUP}'!!!");
                        return;
                    }
                }
                catch (System.Exception ex)
                {
                    DebugLog.Error($"Exception creating Addressable group: {ex.Message}. Please manually initialize Addressables first!!!");
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
                    entry.address = Common.FMOD_REF_SO_NAME;
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
                    DebugLog.Success($"Made ScriptableObject '{assetName}' addressable with address '{entry.address}' in group '{group.Name}'!!!");
                }
                else
                {
                    DebugLog.Error($"Failed to create or move Addressable entry for {assetName} (GUID: {guid})!!!");
                }
            }
            catch (System.Exception ex)
            {
                DebugLog.Error($"Exception creating Addressable entry: {ex.Message}!!!");
            }
        }
    }
}
