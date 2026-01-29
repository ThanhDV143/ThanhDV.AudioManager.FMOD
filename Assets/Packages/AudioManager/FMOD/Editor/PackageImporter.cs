#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using AddressableGroupSchemas = UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace ThanhDV.AudioManager.FMOD
{
    [InitializeOnLoad]
    public static class PackageImporter
    {
        static PackageImporter()
        {
            if (SessionState.GetBool(Common.SESSION_KEY_CHECKED, false)) return;

            if (IsInitializedCorrectly())
            {
                SessionState.SetBool(Common.SESSION_KEY_CHECKED, true);
                return;
            }

            MakeAddressable();
            SessionState.SetBool(Common.SESSION_KEY_CHECKED, IsInitializedCorrectly());
        }

        public static bool IsInitializedCorrectly()
        {
            string path = FindSOPath();
            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid)) return false;

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return false;

            AddressableAssetGroup group = settings.FindGroup(Common.ADDRESSABLE_GROUP);
            if (group == null) return false;

            AddressableAssetEntry entry = settings.FindAssetEntry(guid);
            if (entry == null) return false;

            if (entry.parentGroup.Name != Common.ADDRESSABLE_GROUP) return false;

            return true;
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
            string assetName = $"{Common.FMOD_REF_SO_NAME}{Common.SO_EXTENSION}";
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
#endif