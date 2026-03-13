using FMOD.Studio;

namespace ThanhDV.AudioManager.FMOD
{
    public static class FMODBus
    {
        private static Bus? _cachedSFX_Character;
        public static Bus SFX_Character
        {
            get
            {
                if (_cachedSFX_Character.HasValue && _cachedSFX_Character.Value.isValid()) return _cachedSFX_Character.Value;
                return LoadAndCache("SFX_Character", ref _cachedSFX_Character);
            }
        }

        private static Bus? _cachedSFX_Ambience;
        public static Bus SFX_Ambience
        {
            get
            {
                if (_cachedSFX_Ambience.HasValue && _cachedSFX_Ambience.Value.isValid()) return _cachedSFX_Ambience.Value;
                return LoadAndCache("SFX_Ambience", ref _cachedSFX_Ambience);
            }
        }

        private static Bus? _cachedSFX_Explosions;
        public static Bus SFX_Explosions
        {
            get
            {
                if (_cachedSFX_Explosions.HasValue && _cachedSFX_Explosions.Value.isValid()) return _cachedSFX_Explosions.Value;
                return LoadAndCache("SFX_Explosions", ref _cachedSFX_Explosions);
            }
        }

        private static Bus? _cachedVO;
        public static Bus VO
        {
            get
            {
                if (_cachedVO.HasValue && _cachedVO.Value.isValid()) return _cachedVO.Value;
                return LoadAndCache("VO", ref _cachedVO);
            }
        }

        private static Bus? _cachedSFX_Vehicles;
        public static Bus SFX_Vehicles
        {
            get
            {
                if (_cachedSFX_Vehicles.HasValue && _cachedSFX_Vehicles.Value.isValid()) return _cachedSFX_Vehicles.Value;
                return LoadAndCache("SFX_Vehicles", ref _cachedSFX_Vehicles);
            }
        }

        private static Bus? _cachedSFX_Weapons;
        public static Bus SFX_Weapons
        {
            get
            {
                if (_cachedSFX_Weapons.HasValue && _cachedSFX_Weapons.Value.isValid()) return _cachedSFX_Weapons.Value;
                return LoadAndCache("SFX_Weapons", ref _cachedSFX_Weapons);
            }
        }

        private static Bus? _cachedSFX;
        public static Bus SFX
        {
            get
            {
                if (_cachedSFX.HasValue && _cachedSFX.Value.isValid()) return _cachedSFX.Value;
                return LoadAndCache("SFX", ref _cachedSFX);
            }
        }

        private static Bus? _cachedMusic;
        public static Bus Music
        {
            get
            {
                if (_cachedMusic.HasValue && _cachedMusic.Value.isValid()) return _cachedMusic.Value;
                return LoadAndCache("Music", ref _cachedMusic);
            }
        }

        private static Bus? _cachedSFX_Objects;
        public static Bus SFX_Objects
        {
            get
            {
                if (_cachedSFX_Objects.HasValue && _cachedSFX_Objects.Value.isValid()) return _cachedSFX_Objects.Value;
                return LoadAndCache("SFX_Objects", ref _cachedSFX_Objects);
            }
        }

        private static Bus? _cachedUI;
        public static Bus UI
        {
            get
            {
                if (_cachedUI.HasValue && _cachedUI.Value.isValid()) return _cachedUI.Value;
                return LoadAndCache("UI", ref _cachedUI);
            }
        }

        private static Bus? _cachedSFX_Reverb;
        public static Bus SFX_Reverb
        {
            get
            {
                if (_cachedSFX_Reverb.HasValue && _cachedSFX_Reverb.Value.isValid()) return _cachedSFX_Reverb.Value;
                return LoadAndCache("SFX_Reverb", ref _cachedSFX_Reverb);
            }
        }

        private static Bus? _cachedMaster;
        public static Bus Master
        {
            get
            {
                if (_cachedMaster.HasValue && _cachedMaster.Value.isValid()) return _cachedMaster.Value;
                return LoadAndCache("Master", ref _cachedMaster);
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
