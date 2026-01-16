using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace ThanhDV.AudioManager.FMOD
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] private EventReference _bgmEventReference;
        [SerializeField] private EventReference _footEventReference;
        [SerializeField] private EventReference _loopReference;
        [SerializeField] private EventReference _oneShot;

        void Start()
        {
            // AudioManager.Instance.PlayBGM("event:/Music/Level 02");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                AudioManager.Instance.PlayOneShot(_oneShot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                AudioManager.Instance.TryGetEventInstance("Loop", out EventInstance instance);
                instance.setPitch(2f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                AudioManager.Instance.TryGetEventInstance("Loop", out EventInstance instance);
                instance.setPitch(0.5f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                AudioManager.Instance.StopBGM();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                AudioManager.Instance.PlayLoop("X", _footEventReference, gameObject);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                AudioManager.Instance.StopLoop("X");
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Destroy(AudioManager.Instance.gameObject);
            }

            // if (Input.GetKeyDown(KeyCode.Alpha8))
            // {
            //     float volume = AudioManager.Instance.GetVolume(AudioType.MASTER);
            //     AudioManager.Instance.SetVolume(AudioType.MASTER, volume <= 0 ? 1f : 0f);
            // }

            // if (Input.GetKeyDown(KeyCode.Alpha9))
            // {
            //     float volume = AudioManager.Instance.GetVolume(AudioType.BGM);
            //     AudioManager.Instance.SetVolume(AudioType.BGM, volume <= 0 ? 1f : 0f);
            // }

            // if (Input.GetKeyDown(KeyCode.Alpha0))
            // {
            //     float volume = AudioManager.Instance.GetVolume(AudioType.SFX);
            //     AudioManager.Instance.SetVolume(AudioType.SFX, volume <= 0 ? 1f : 0f);
            // }
        }
    }
}
