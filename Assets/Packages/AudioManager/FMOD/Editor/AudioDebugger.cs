// #if UNITY_EDITOR
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace ThanhDV.AudioManager.FMOD
// {
//     public class AudioDebugger : MonoBehaviour
//     {

//         [Header("Volume")]
//         [SerializeField, Range(0f, 1f)] private float masterVolume;
//         [SerializeField, Range(0f, 1f)] private float bgmVolume;
//         [SerializeField, Range(0f, 1f)] private float sfxVolume;

//         private void Awake()
//         {
//             InitializeDebug();
//         }

//         private void InitializeDebug()
//         {
//             masterVolume = GetVolume(AudioType.MASTER);
//             bgmVolume = GetVolume(AudioType.BGM);
//             sfxVolume = GetVolume(AudioType.SFX);
//         }

//         private void OnValidate()
//         {
//             SetVolume(AudioType.MASTER, masterVolume);
//             SetVolume(AudioType.BGM, bgmVolume);
//             SetVolume(AudioType.SFX, sfxVolume);
//         }

//         private void Update()
//         {
//             masterVolume = GetVolume(AudioType.MASTER);
//             bgmVolume = GetVolume(AudioType.BGM);
//             sfxVolume = GetVolume(AudioType.SFX);
//         }

//         public void SetVolume(AudioType type, float volume)
//         {
//             if (!audioBuses.TryGetValue(type, out Bus bus))
//             {
//                 Debug.Log($"<color=red>[AudioManager - FMOD] Bus not found: '{type.ToString()}'. Please check your FMOD Studio project or Initialize first!!!</color>");
//                 return;
//             }

//             volume = Mathf.Clamp01(volume);
//             bus.setVolume(volume);
//         }

//         public float GetVolume(AudioType type)
//         {
//             if (!audioBuses.TryGetValue(type, out Bus bus))
//             {
//                 Debug.Log($"<color=red>[AudioManager - FMOD] Bus not found: '{type.ToString()}'. Please check your FMOD Studio project or Initialize first!!!</color>");
//                 return -1f;
//             }

//             bus.getVolume(out float volume);
//             return volume;
//         }
//     }
// }
// #endif
