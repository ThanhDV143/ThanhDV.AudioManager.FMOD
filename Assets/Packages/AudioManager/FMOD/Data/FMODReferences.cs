using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public class FMODReferences : ScriptableObject
    {
        [SerializeField] private List<BusEntry> _buses = new(); public List<BusEntry> Buses => _buses;
        [SerializeField] private List<EventReferenceEntry> _eventReferences = new(); public List<EventReferenceEntry> EventReferences => _eventReferences;

        private Dictionary<string, Bus> _cachedBuses;
        private Dictionary<string, EventReference> _cachedEventReferences;

        public Bus GetBus(string key)
        {
            if (_cachedBuses == null)
            {
                InitializeBusCache();
            }

            if (_cachedBuses.TryGetValue(key, out Bus bus)) return bus;

            DebugLog.Error($"Bus with key '{key}' not found!!!");
            return default;
        }

        public bool TryGetEventReference(string key, out EventReference eventReference)
        {
            if (_cachedEventReferences == null)
            {
                InitializeEventReferenceCache();
            }

            if (_cachedEventReferences.TryGetValue(key, out eventReference)) return true;

            DebugLog.Error($"EventReference with key '{key}' not found!!!");
            return false;
        }

        private void InitializeBusCache()
        {
            _cachedBuses = new Dictionary<string, Bus>();
            foreach (BusEntry entry in _buses)
            {
                if (string.IsNullOrEmpty(entry.Key)) continue;

                if (_cachedBuses.ContainsKey(entry.Key))
                {
                    DebugLog.Warning($"Duplicate Bus key '{entry.Key}' in FMODReferences.");
                    continue;
                }

                _cachedBuses.Add(entry.Key, RuntimeManager.GetBus(entry.BusPath));
            }
        }

        private void InitializeEventReferenceCache()
        {
            _cachedEventReferences = new Dictionary<string, EventReference>();
            foreach (EventReferenceEntry entry in _eventReferences)
            {
                if (string.IsNullOrEmpty(entry.Key)) continue;

                if (_cachedEventReferences.ContainsKey(entry.Key))
                {
                    DebugLog.Warning($"Duplicate EventReference key '{entry.Key}' in FMODReferences.");
                    continue;
                }

                _cachedEventReferences.Add(entry.Key, entry.EventReference);
            }
        }
    }
}
