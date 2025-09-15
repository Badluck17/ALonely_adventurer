using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Zycalipse.Managers
{
    public static class SoundManager
    {
        public static async void PlayOneShotSound(AudioClip audioClip, Vector3 position)
        {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.transform.position = position;

            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(audioClip);
            OneShotObjectHandler(audioSource);
        }

        private static async void OneShotObjectHandler(AudioSource audioSource)
        {
            while (audioSource.isPlaying)
            {
                await Task.Yield();
            }

            Object.Destroy(audioSource.gameObject);
        }
    }
}
