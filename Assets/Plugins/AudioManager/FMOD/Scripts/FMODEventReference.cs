using FMODUnity;

namespace ThanhDV.AudioManager.FMOD
{
    public static class FMODEventReference
    {
        private static EventReference? _cachedsnapshot__Slow_Motion;
        public static EventReference snapshot__Slow_Motion
        {
            get
            {
                if (_cachedsnapshot__Slow_Motion.HasValue && !_cachedsnapshot__Slow_Motion.Value.IsNull) return _cachedsnapshot__Slow_Motion.Value;
                return LoadAndCache("snapshot__Slow_Motion", ref _cachedsnapshot__Slow_Motion);
            }
        }

        private static EventReference? _cachedsnapshot__Health_Low;
        public static EventReference snapshot__Health_Low
        {
            get
            {
                if (_cachedsnapshot__Health_Low.HasValue && !_cachedsnapshot__Health_Low.Value.IsNull) return _cachedsnapshot__Health_Low.Value;
                return LoadAndCache("snapshot__Health_Low", ref _cachedsnapshot__Health_Low);
            }
        }

        private static EventReference? _cachedsnapshot__Reverb_Sewer_Reverb;
        public static EventReference snapshot__Reverb_Sewer_Reverb
        {
            get
            {
                if (_cachedsnapshot__Reverb_Sewer_Reverb.HasValue && !_cachedsnapshot__Reverb_Sewer_Reverb.Value.IsNull) return _cachedsnapshot__Reverb_Sewer_Reverb.Value;
                return LoadAndCache("snapshot__Reverb_Sewer_Reverb", ref _cachedsnapshot__Reverb_Sewer_Reverb);
            }
        }

        private static EventReference? _cachedsnapshot__Pause;
        public static EventReference snapshot__Pause
        {
            get
            {
                if (_cachedsnapshot__Pause.HasValue && !_cachedsnapshot__Pause.Value.IsNull) return _cachedsnapshot__Pause.Value;
                return LoadAndCache("snapshot__Pause", ref _cachedsnapshot__Pause);
            }
        }

        private static EventReference? _cachedsnapshot__Reverb_Cave_Reverb;
        public static EventReference snapshot__Reverb_Cave_Reverb
        {
            get
            {
                if (_cachedsnapshot__Reverb_Cave_Reverb.HasValue && !_cachedsnapshot__Reverb_Cave_Reverb.Value.IsNull) return _cachedsnapshot__Reverb_Cave_Reverb.Value;
                return LoadAndCache("snapshot__Reverb_Cave_Reverb", ref _cachedsnapshot__Reverb_Cave_Reverb);
            }
        }

        private static EventReference? _cachedMusic_Level_03;
        public static EventReference Music_Level_03
        {
            get
            {
                if (_cachedMusic_Level_03.HasValue && !_cachedMusic_Level_03.Value.IsNull) return _cachedMusic_Level_03.Value;
                return LoadAndCache("Music_Level_03", ref _cachedMusic_Level_03);
            }
        }

        private static EventReference? _cachedMusic_Level_01;
        public static EventReference Music_Level_01
        {
            get
            {
                if (_cachedMusic_Level_01.HasValue && !_cachedMusic_Level_01.Value.IsNull) return _cachedMusic_Level_01.Value;
                return LoadAndCache("Music_Level_01", ref _cachedMusic_Level_01);
            }
        }

        private static EventReference? _cachedMusic_Radio_Station;
        public static EventReference Music_Radio_Station
        {
            get
            {
                if (_cachedMusic_Radio_Station.HasValue && !_cachedMusic_Radio_Station.Value.IsNull) return _cachedMusic_Radio_Station.Value;
                return LoadAndCache("Music_Radio_Station", ref _cachedMusic_Radio_Station);
            }
        }

        private static EventReference? _cachedMusic_Level_02;
        public static EventReference Music_Level_02
        {
            get
            {
                if (_cachedMusic_Level_02.HasValue && !_cachedMusic_Level_02.Value.IsNull) return _cachedMusic_Level_02.Value;
                return LoadAndCache("Music_Level_02", ref _cachedMusic_Level_02);
            }
        }

        private static EventReference? _cachedCharacter_Player_Footsteps;
        public static EventReference Character_Player_Footsteps
        {
            get
            {
                if (_cachedCharacter_Player_Footsteps.HasValue && !_cachedCharacter_Player_Footsteps.Value.IsNull) return _cachedCharacter_Player_Footsteps.Value;
                return LoadAndCache("Character_Player_Footsteps", ref _cachedCharacter_Player_Footsteps);
            }
        }

        private static EventReference? _cachedCharacter_Door_Open;
        public static EventReference Character_Door_Open
        {
            get
            {
                if (_cachedCharacter_Door_Open.HasValue && !_cachedCharacter_Door_Open.Value.IsNull) return _cachedCharacter_Door_Open.Value;
                return LoadAndCache("Character_Door_Open", ref _cachedCharacter_Door_Open);
            }
        }

        private static EventReference? _cachedWeapons_Explosion;
        public static EventReference Weapons_Explosion
        {
            get
            {
                if (_cachedWeapons_Explosion.HasValue && !_cachedWeapons_Explosion.Value.IsNull) return _cachedWeapons_Explosion.Value;
                return LoadAndCache("Weapons_Explosion", ref _cachedWeapons_Explosion);
            }
        }

        private static EventReference? _cachedCharacter_Enemy_Footsteps;
        public static EventReference Character_Enemy_Footsteps
        {
            get
            {
                if (_cachedCharacter_Enemy_Footsteps.HasValue && !_cachedCharacter_Enemy_Footsteps.Value.IsNull) return _cachedCharacter_Enemy_Footsteps.Value;
                return LoadAndCache("Character_Enemy_Footsteps", ref _cachedCharacter_Enemy_Footsteps);
            }
        }

        private static EventReference? _cachedInteractables_Barrel_Roll;
        public static EventReference Interactables_Barrel_Roll
        {
            get
            {
                if (_cachedInteractables_Barrel_Roll.HasValue && !_cachedInteractables_Barrel_Roll.Value.IsNull) return _cachedInteractables_Barrel_Roll.Value;
                return LoadAndCache("Interactables_Barrel_Roll", ref _cachedInteractables_Barrel_Roll);
            }
        }

        private static EventReference? _cachedCharacter_Door_Close;
        public static EventReference Character_Door_Close
        {
            get
            {
                if (_cachedCharacter_Door_Close.HasValue && !_cachedCharacter_Door_Close.Value.IsNull) return _cachedCharacter_Door_Close.Value;
                return LoadAndCache("Character_Door_Close", ref _cachedCharacter_Door_Close);
            }
        }

        private static EventReference? _cachedAmbience_Country;
        public static EventReference Ambience_Country
        {
            get
            {
                if (_cachedAmbience_Country.HasValue && !_cachedAmbience_Country.Value.IsNull) return _cachedAmbience_Country.Value;
                return LoadAndCache("Ambience_Country", ref _cachedAmbience_Country);
            }
        }

        private static EventReference? _cachedAmbience_City;
        public static EventReference Ambience_City
        {
            get
            {
                if (_cachedAmbience_City.HasValue && !_cachedAmbience_City.Value.IsNull) return _cachedAmbience_City.Value;
                return LoadAndCache("Ambience_City", ref _cachedAmbience_City);
            }
        }

        private static EventReference? _cachedCharacter_Radio_Dialogue;
        public static EventReference Character_Radio_Dialogue
        {
            get
            {
                if (_cachedCharacter_Radio_Dialogue.HasValue && !_cachedCharacter_Radio_Dialogue.Value.IsNull) return _cachedCharacter_Radio_Dialogue.Value;
                return LoadAndCache("Character_Radio_Dialogue", ref _cachedCharacter_Radio_Dialogue);
            }
        }

        private static EventReference? _cachedUI_Okay;
        public static EventReference UI_Okay
        {
            get
            {
                if (_cachedUI_Okay.HasValue && !_cachedUI_Okay.Value.IsNull) return _cachedUI_Okay.Value;
                return LoadAndCache("UI_Okay", ref _cachedUI_Okay);
            }
        }

        private static EventReference? _cachedCharacter_Dialogue;
        public static EventReference Character_Dialogue
        {
            get
            {
                if (_cachedCharacter_Dialogue.HasValue && !_cachedCharacter_Dialogue.Value.IsNull) return _cachedCharacter_Dialogue.Value;
                return LoadAndCache("Character_Dialogue", ref _cachedCharacter_Dialogue);
            }
        }

        private static EventReference? _cachedWeapons_Pistol;
        public static EventReference Weapons_Pistol
        {
            get
            {
                if (_cachedWeapons_Pistol.HasValue && !_cachedWeapons_Pistol.Value.IsNull) return _cachedWeapons_Pistol.Value;
                return LoadAndCache("Weapons_Pistol", ref _cachedWeapons_Pistol);
            }
        }

        private static EventReference? _cachedCharacter_Health;
        public static EventReference Character_Health
        {
            get
            {
                if (_cachedCharacter_Health.HasValue && !_cachedCharacter_Health.Value.IsNull) return _cachedCharacter_Health.Value;
                return LoadAndCache("Character_Health", ref _cachedCharacter_Health);
            }
        }

        private static EventReference? _cachedInteractables_Wooden_Collision;
        public static EventReference Interactables_Wooden_Collision
        {
            get
            {
                if (_cachedInteractables_Wooden_Collision.HasValue && !_cachedInteractables_Wooden_Collision.Value.IsNull) return _cachedInteractables_Wooden_Collision.Value;
                return LoadAndCache("Interactables_Wooden_Collision", ref _cachedInteractables_Wooden_Collision);
            }
        }

        private static EventReference? _cachedAmbience_Forest;
        public static EventReference Ambience_Forest
        {
            get
            {
                if (_cachedAmbience_Forest.HasValue && !_cachedAmbience_Forest.Value.IsNull) return _cachedAmbience_Forest.Value;
                return LoadAndCache("Ambience_Forest", ref _cachedAmbience_Forest);
            }
        }

        private static EventReference? _cachedUI_Cancel;
        public static EventReference UI_Cancel
        {
            get
            {
                if (_cachedUI_Cancel.HasValue && !_cachedUI_Cancel.Value.IsNull) return _cachedUI_Cancel.Value;
                return LoadAndCache("UI_Cancel", ref _cachedUI_Cancel);
            }
        }

        private static EventReference? _cachedWeapons_Machine_Gun;
        public static EventReference Weapons_Machine_Gun
        {
            get
            {
                if (_cachedWeapons_Machine_Gun.HasValue && !_cachedWeapons_Machine_Gun.Value.IsNull) return _cachedWeapons_Machine_Gun.Value;
                return LoadAndCache("Weapons_Machine_Gun", ref _cachedWeapons_Machine_Gun);
            }
        }

        private static EventReference? _cachedVO_Main_Menu;
        public static EventReference VO_Main_Menu
        {
            get
            {
                if (_cachedVO_Main_Menu.HasValue && !_cachedVO_Main_Menu.Value.IsNull) return _cachedVO_Main_Menu.Value;
                return LoadAndCache("VO_Main_Menu", ref _cachedVO_Main_Menu);
            }
        }

        private static EventReference? _cachedVO_Welcome;
        public static EventReference VO_Welcome
        {
            get
            {
                if (_cachedVO_Welcome.HasValue && !_cachedVO_Welcome.Value.IsNull) return _cachedVO_Welcome.Value;
                return LoadAndCache("VO_Welcome", ref _cachedVO_Welcome);
            }
        }

        private static EventReference? _cachedVehicles_Car_Engine;
        public static EventReference Vehicles_Car_Engine
        {
            get
            {
                if (_cachedVehicles_Car_Engine.HasValue && !_cachedVehicles_Car_Engine.Value.IsNull) return _cachedVehicles_Car_Engine.Value;
                return LoadAndCache("Vehicles_Car_Engine", ref _cachedVehicles_Car_Engine);
            }
        }

        private static EventReference? _cachedVehicles_Ride_on_Mower;
        public static EventReference Vehicles_Ride_on_Mower
        {
            get
            {
                if (_cachedVehicles_Ride_on_Mower.HasValue && !_cachedVehicles_Ride_on_Mower.Value.IsNull) return _cachedVehicles_Ride_on_Mower.Value;
                return LoadAndCache("Vehicles_Ride_on_Mower", ref _cachedVehicles_Ride_on_Mower);
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
