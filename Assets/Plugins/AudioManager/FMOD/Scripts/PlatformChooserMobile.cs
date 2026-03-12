using UnityEditor;

namespace ThanhDV.AudioManager.FMOD
{
    public class PlatformChooserMobile
    {
        [MenuItem(Common.MENU_ITEM + "Platform/Mobile", false, 0)]
        public static void Choose()
        {
            PlatformChooser.ChangeDefaultProjectPlatform("Mobile");
        }

        [MenuItem(Common.MENU_ITEM + "Platform/Mobile", true)]
        public static bool ValidateChoose()
        {
            string menuPath = Common.MENU_ITEM + "Platform/Mobile";
            Menu.SetChecked(menuPath, PlatformChooser.IsDefaultPlatform("Mobile"));
            return true;
        }
    }
}
