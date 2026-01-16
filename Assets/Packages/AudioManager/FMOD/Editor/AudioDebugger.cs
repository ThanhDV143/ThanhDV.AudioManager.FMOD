#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ThanhDV.AudioManager.FMOD
{
    public class AudioDebugger : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] private List<AudioVolume> _audioVolumes = new();

        private FMODReferences _fMODReferences; public FMODReferences FMODReferences => _fMODReferences;
        private AsyncOperationHandle<FMODReferences> _fMODReferencesHandle;
        private bool _hasSaveSettingsHandle;

        private async void Awake()
        {
            await TryLoadFMODReferences();
            InitializeDebug();
        }

        private void InitializeDebug()
        {
            _audioVolumes = new();
            List<BusEntry> busEntries = new(_fMODReferences.GetBuses());
            BusEntry busEntry;

            for (int i = 0; i < busEntries.Count; i++)
            {
                busEntry = busEntries[i];
                _audioVolumes.Add(new(busEntry.Key, GetVolume(busEntry.Key)));
            }
        }

        private void OnValidate()
        {
            SetVolumes();
        }

        private async Task TryLoadFMODReferences()
        {
            if (_fMODReferences != null) return;
            try
            {
                _fMODReferencesHandle = Addressables.LoadAssetAsync<FMODReferences>(Common.FMOD_REF_SO_NAME);
                _hasSaveSettingsHandle = true;
                _fMODReferences = await _fMODReferencesHandle.Task;

                if (_fMODReferences == null)
                {
                    DebugLog.Error($"Could not load FMODReferences from Addressables. A default FMODReferences object will be created. Please ensure a '{Common.FMOD_REF_SO_NAME}' asset exists and is configured in Addressables!!!");
                    _fMODReferences = ScriptableObject.CreateInstance<FMODReferences>();

                    if (_fMODReferencesHandle.IsValid())
                        Addressables.Release(_fMODReferencesHandle);
                    _hasSaveSettingsHandle = false;
                }
            }
            catch (Exception e)
            {
                DebugLog.Error($"Could not load FMODReferences from Addressables. A default FMODReferences object will be created. Please ensure a '{Common.FMOD_REF_SO_NAME}' asset exists and is configured in Addressables!!!\n{e}");
                _fMODReferences = _fMODReferences != null ? _fMODReferences : ScriptableObject.CreateInstance<FMODReferences>();

                if (_hasSaveSettingsHandle && _fMODReferencesHandle.IsValid()) Addressables.Release(_fMODReferencesHandle);
                _hasSaveSettingsHandle = false;
            }
        }

        private void Update()
        {
            GetVolumes();
        }

        private void GetVolumes()
        {
            AudioVolume audioVolume;

            for (int i = 0; i < _audioVolumes.Count; i++)
            {
                audioVolume = _audioVolumes[i];
                audioVolume.Volume = GetVolume(audioVolume.Key);
            }
        }

        private void SetVolumes()
        {
            AudioVolume audioVolume;

            for (int i = 0; i < _audioVolumes.Count; i++)
            {
                audioVolume = _audioVolumes[i];
                SetVolume(audioVolume.Key, audioVolume.Volume);
            }
        }

        public void SetVolume(string busKey, float volume)
        {
            if (string.IsNullOrEmpty(busKey) || string.IsNullOrWhiteSpace(busKey))
            {
                DebugLog.Error("Cannot set volume: The provided eventReferencePath is null.");
                return;
            }

            volume = Mathf.Clamp01(volume);

            Bus bus = _fMODReferences.GetBus(busKey);
            bus.setVolume(volume);
        }

        public float GetVolume(string busKey)
        {
            if (string.IsNullOrEmpty(busKey) || string.IsNullOrWhiteSpace(busKey))
            {
                DebugLog.Error("Cannot get volume: The provided eventReferencePath is null.");
                return -1;
            }

            _fMODReferences.GetBus(busKey).getVolume(out float volume);
            return volume;
        }

        [Serializable]
        private class AudioVolume
        {
            public string Key;
            [Range(0f, 1f)] public float Volume;

            public AudioVolume(string key, float volume)
            {
                Key = key;
                Volume = volume;
            }
        }
    }
}
#endif
