using FMOD.Studio;

namespace ThanhDV.AudioManager.FMOD
{
    public static class FMODBus
    {
        private static Bus? _cachedB;
        public static Bus B
        {
            get
            {
                if (_cachedB.HasValue && _cachedB.Value.isValid()) return _cachedB.Value;
                return LoadAndCache("B", ref _cachedB);
            }
        }

        private static Bus? _cachedC;
        public static Bus C
        {
            get
            {
                if (_cachedC.HasValue && _cachedC.Value.isValid()) return _cachedC.Value;
                return LoadAndCache("C", ref _cachedC);
            }
        }

        private static Bus? _cachedA;
        public static Bus A
        {
            get
            {
                if (_cachedA.HasValue && _cachedA.Value.isValid()) return _cachedA.Value;
                return LoadAndCache("A", ref _cachedA);
            }
        }

        private static Bus LoadAndCache(string key, ref Bus? cacheField)
        {
            if (!AudioManager.IsExist || AudioManager.Instance.FMODReferences == null)
            {
                DebugLog.Warning($"AudioManager is not initialized or FMODReferences is null. Cannot resolve bus key '{key}'.");
                return default;
            }

            Bus result = AudioManager.Instance.FMODReferences.GetBus(key);

            if (result.isValid()) cacheField = result;

            return result;
        }
    }
}
