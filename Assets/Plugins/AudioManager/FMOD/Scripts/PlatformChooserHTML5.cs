using UnityEditor;

namespace ThanhDV.AudioManager.FMOD
{
    public class PlatformChooserHTML5
    {
        [MenuItem(Common.MENU_ITEM + "Platform/HTML5", false, 0)]
        public static void Choose()
        {
            PlatformChooser.ChangeDefaultProjectPlatform("HTML5");
        }

        [MenuItem(Common.MENU_ITEM + "Platform/HTML5", true)]
        public static bool ValidateChoose()
        {
            string menuPath = Common.MENU_ITEM + "Platform/HTML5";
            Menu.SetChecked(menuPath, PlatformChooser.IsDefaultPlatform("HTML5"));
            return true;
        }
    }
}
