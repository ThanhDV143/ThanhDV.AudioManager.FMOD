namespace ThanhDV.AudioManager.FMOD
{
    public static class Common
    {
        public const string FMOD_REF_SO_FOLDER = "Assets/Plugins/AudioManager/FMOD/SO";
        public const string FMOD_REF_SCRIPT_FOLDER = "Assets/Plugins/AudioManager/FMOD/Scripts";

        public const string FMOD_BUS_SCRIPT_NAME = "FMODBus";
        public const string FMOD_EVENT_REF_SCRIPT_NAME = "FMODEventReference";
        public const string SCRIPT_EXTENSION = ".cs";
        public const string FMOD_BUS_SCRIPT_PATH = FMOD_REF_SCRIPT_FOLDER + "/" + FMOD_BUS_SCRIPT_NAME + SCRIPT_EXTENSION;
        public const string FMOD_EVENT_REF_SCRIPT_PATH = FMOD_REF_SCRIPT_FOLDER + "/" + FMOD_EVENT_REF_SCRIPT_NAME + SCRIPT_EXTENSION;

        public const string FMOD_REF_SO_NAME = "FMODReferences";
        public const string SO_EXTENSION = ".asset";
        public const string FMOD_REF_SO_PATH = FMOD_REF_SO_FOLDER + "/" + FMOD_REF_SO_NAME + SO_EXTENSION;


        public const string ADDRESSABLE_GROUP = "AudioManager";

        public const string SESSION_KEY_CHECKED = "ThanhDV.AudioManager.FMOD.CheckedInSession";

        public const string MENU_ITEM = "Tools/ThanhDV/Audio Manager - FMOD/";
    }
}
