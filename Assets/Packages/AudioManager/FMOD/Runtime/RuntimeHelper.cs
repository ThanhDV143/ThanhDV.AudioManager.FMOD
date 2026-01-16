using FMODUnity;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public static class RuntimeHelper
    {
        public static bool IsNull(this EventReference eventReference)
        {
            if (eventReference.IsNull)
            {
                DebugLog.Error("Cannot play sound: The provided EventReference is null.");
                return true;
            }

            return false;
        }

        public static bool IsEventReferenceNull(this string eventReferencePath)
        {
            if (string.IsNullOrEmpty(eventReferencePath) || string.IsNullOrWhiteSpace(eventReferencePath))
            {
                DebugLog.Error("Cannot play sound: The provided eventReferencePath is null.");
                return true;
            }

            return false;
        }
    }
}
