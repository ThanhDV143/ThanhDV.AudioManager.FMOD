#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ThanhDV.AudioManager.FMOD
{
    internal static class FMODReferencesEditorAsset
    {
        private const string LoadingMessage = "Loading FMODReferences. Please wait a moment...";
        private const string NotFoundMessage = "FMODReferences asset not found. Please initialize it first.";
        private const string ButtonLabel = "Find or Create FMODReferences";

        public static bool DrawEnsureFMODReferencesUI(bool isLoading, FMODReferences fmodReferences, System.Action requestLoadOrCreate, string loadingMessage = LoadingMessage, string notFoundMessage = NotFoundMessage, string buttonLabel = ButtonLabel)
        {
            if (isLoading)
            {
                EditorGUILayout.HelpBox(loadingMessage, MessageType.Info);
                return false;
            }

            if (fmodReferences == null)
            {
                EditorGUILayout.HelpBox(notFoundMessage, MessageType.Warning);
                if (requestLoadOrCreate != null && GUILayout.Button(buttonLabel))
                {
                    requestLoadOrCreate.Invoke();
                }
                return false;
            }

            return true;
        }

        public static async Task<FMODReferences> LoadOrCreateAsync(FMODReferences current)
        {
            if (current != null) return current;

            FMODReferences addressable = await TryLoadAddressableAsync();
            if (addressable != null) return addressable;

            return LoadFromAssetDatabaseOrCreate();
        }

        public static FMODReferences LoadFromAssetDatabaseOrCreate()
        {
            string path = FindOrCreatePath();
            return AssetDatabase.LoadAssetAtPath<FMODReferences>(path);
        }

        public static string FindOrCreatePath()
        {
            if (AssetDatabase.LoadAssetAtPath<FMODReferences>(Common.FMOD_REF_SO_PATH) != null)
                return Common.FMOD_REF_SO_PATH;

            string[] guids = AssetDatabase.FindAssets($"{Common.FMOD_REF_SO_NAME} t:{nameof(FMODReferences)}");
            if (guids == null || guids.Length == 0)
                guids = AssetDatabase.FindAssets($"t:{nameof(FMODReferences)}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.LoadAssetAtPath<FMODReferences>(path) != null)
                    return path;
            }

            return CreateAtDefaultPath();
        }

        private static async Task<FMODReferences> TryLoadAddressableAsync()
        {
            var handle = Addressables.LoadAssetAsync<FMODReferences>(Common.FMOD_REF_SO_NAME);
            try
            {
                return await handle.Task;
            }
            finally
            {
                handle.Release();
            }
        }

        private static string CreateAtDefaultPath()
        {
            EditorHelper.EnsureFolderPath(Common.FMOD_REF_SO_FOLDER);

            ScriptableObject existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(Common.FMOD_REF_SO_PATH);
            if (existing != null) return Common.FMOD_REF_SO_PATH;

            FMODReferences instance = ScriptableObject.CreateInstance<FMODReferences>();
            AssetDatabase.CreateAsset(instance, Common.FMOD_REF_SO_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            DebugLog.Warning($"Auto-created FMODReferences at {Common.FMOD_REF_SO_PATH}");
            return Common.FMOD_REF_SO_PATH;
        }
    }
}
#endif