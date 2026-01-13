// Copyright (c) 2023 Iain McManus

using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThanhDV.AudioManager.FMOD
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<TKey> SerializedKeys = new();
        public List<TValue> SerializedValues = new();

        public void OnAfterDeserialize()
        {
            SynchroniseToSerializedData();
        }

        public void OnBeforeSerialize() { }

#if UNITY_EDITOR
        public void EditorOnly_Add(TKey InKey, TValue InValue)
        {
            SerializedKeys.Add(InKey);
            SerializedValues.Add(InValue);
        }
#endif // UNITY_EDITOR

        public void SynchroniseToSerializedData()
        {
            this.Clear();

            // if we have valid data then build the dictionary
            if ((SerializedKeys != null) && (SerializedValues != null))
            {
                int NumElements = Mathf.Min(SerializedKeys.Count, SerializedValues.Count);
                for (int Index = 0; Index < NumElements; ++Index)
                {
                    this[SerializedKeys[Index]] = SerializedValues[Index];
                }
            }
            else
            {
                SerializedKeys = new();
                SerializedValues = new();
            }

            // if the lists are out of sync then rebuild
            if (SerializedKeys.Count != SerializedValues.Count)
            {
                SerializedKeys = new(Keys);
                SerializedValues = new(Values);
            }
        }
    }
}
