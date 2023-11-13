using Audio.SFXPlayers;
using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.UIAudio
{
    public class CollectibleUIAudioManager : SFXAudioPlayer
    {
        [Header("Audios")]
        [SerializeField] private AudioCueSO collectibleUsableAudio;

        [SerializeField] private AudioCueSO collectibleAcquired;
        [SerializeField] private AudioCueSO collectibleUsed;
        

        private AudioCueKey m_collectibleUsableKey;

        public void PlayCollectibleUsableSound()
        {
            m_collectibleUsableKey = PlayAudio(collectibleUsableAudio);
        }

        public void StopCollectibleUsableSound()
        {
            StopAudio(m_collectibleUsableKey);
        }

        public void PlayCollectibleAcquired() => PlayAudio(collectibleAcquired);
        public void PlayCollectibleUsed() => PlayAudio(collectibleUsed);
    }
}