using UnityEditor;

namespace ThanhDV.AudioManager.FMOD
{
    public class AudioManagerInitializer : EditorWindow
    {
        [MenuItem(Common.MENU_ITEM + "Initialize", false, 0)]
        public static void Initialize()
        {
            string packageVersion = PackageImporter.GetPackageVersion();
            string editorPrefsKey = $"{Common.EDITOR_PREF_KEY_PREFIX}{packageVersion}";

            if (!EditorPrefs.HasKey(editorPrefsKey)) EditorPrefs.SetBool(editorPrefsKey, false);

            PackageImporter.MakeAddressable();
            EditorPrefs.SetBool(editorPrefsKey, true);
        }
    }
}