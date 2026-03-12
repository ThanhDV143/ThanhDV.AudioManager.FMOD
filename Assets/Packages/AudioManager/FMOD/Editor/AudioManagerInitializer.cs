#if UNITY_EDITOR
using UnityEditor;

namespace ThanhDV.AudioManager.FMOD
{
    public class AudioManagerInitializer
    {
        [MenuItem(Common.MENU_ITEM + "Initialize", false, 0)]
        public static void Initialize()
        {
            if (PackageImporter.IsInitializedCorrectly())
            {
                SessionState.SetBool(Common.SESSION_KEY_CHECKED, true);
                return;
            }

            PackageImporter.MakeAddressable();
            SessionState.SetBool(Common.SESSION_KEY_CHECKED, PackageImporter.IsInitializedCorrectly());
        }
    }
}
#endif