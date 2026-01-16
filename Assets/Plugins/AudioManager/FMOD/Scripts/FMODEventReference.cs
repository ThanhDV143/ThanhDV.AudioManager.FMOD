using FMODUnity;

namespace ThanhDV.AudioManager.FMOD
{
    public static class FMODEventReference
    {
        private static EventReference? _cachedWhat_is_this;
        public static EventReference What_is_this
        {
            get
            {
                if (_cachedWhat_is_this.HasValue && !_cachedWhat_is_this.Value.IsNull) return _cachedWhat_is_this.Value;
                return LoadAndCache("What is this", ref _cachedWhat_is_this);
            }
        }

        private static EventReference? _cachedI_dont_know;
        public static EventReference I_dont_know
        {
            get
            {
                if (_cachedI_dont_know.HasValue && !_cachedI_dont_know.Value.IsNull) return _cachedI_dont_know.Value;
                return LoadAndCache("I dont know", ref _cachedI_dont_know);
            }
        }

        private static EventReference? _cachedTen_minutes_remaining;
        public static EventReference Ten_minutes_remaining
        {
            get
            {
                if (_cachedTen_minutes_remaining.HasValue && !_cachedTen_minutes_remaining.Value.IsNull) return _cachedTen_minutes_remaining.Value;
                return LoadAndCache("Ten minutes remaining", ref _cachedTen_minutes_remaining);
            }
        }

        private static EventReference? _cachedAgain;
        public static EventReference Again
        {
            get
            {
                if (_cachedAgain.HasValue && !_cachedAgain.Value.IsNull) return _cachedAgain.Value;
                return LoadAndCache("Again", ref _cachedAgain);
            }
        }

        private static EventReference LoadAndCache(string key, ref EventReference? cacheField)
        {
            if (!AudioManager.IsExist || AudioManager.Instance.FMODReferences == null)
            {
                DebugLog.Warning($"AudioManager is not initialized or FMODReferences is null. Cannot resolve EventReference key '{key}'.");
                return default;
            }

            EventReference result = AudioManager.Instance.FMODReferences.GetEventReference(key);

            if (!result.IsNull) cacheField = result;

            return result;
        }
    }
}
