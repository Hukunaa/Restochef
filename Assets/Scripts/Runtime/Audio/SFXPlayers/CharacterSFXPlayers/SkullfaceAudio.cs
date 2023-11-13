using System;
using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.SFXPlayers.CharacterSFXPlayers
{
    public class SkullfaceAudio : CharacterAudio
    {
        [SerializeField] private AudioCueSO _hover;
        [SerializeField] private AudioCueSO _boost;
        [SerializeField] private AudioCueSO _teleport;
        [SerializeField] private AudioCueSO _teleportImpossible;
        [SerializeField] private AudioCueSO _preventFallSlowMotion;
        [SerializeField] private AudioCueSO _fall;
        
        private AudioCueKey m_hoverAudioCueKey;
        private AudioCueKey m_boostAudioKey;

        private AudioCueKey m_preventFallSlowMotionKey;
        private AudioCueKey m_fallKey;

        private void Awake()
        {
            m_hoverAudioCueKey = AudioCueKey.Invalid;
            m_boostAudioKey = AudioCueKey.Invalid;
            m_preventFallSlowMotionKey = AudioCueKey.Invalid;
            m_fallKey = AudioCueKey.Invalid;
        }

        public void PlayHoverSound()
        {
            if (m_hoverAudioCueKey != AudioCueKey.Invalid) return;
            m_hoverAudioCueKey = PlayAudio(_hover, transform.position);
        }

        public void StopHoverSound()
        {
            if (m_hoverAudioCueKey == AudioCueKey.Invalid) return;
            StopAudio(m_hoverAudioCueKey);
            m_hoverAudioCueKey = AudioCueKey.Invalid;
        }

        public void PlayBoostSound()
        {
            if (m_boostAudioKey != AudioCueKey.Invalid) return;
            m_boostAudioKey = PlayAudio(_boost, Vector3.zero);
        }

        public void StopBoostSound()
        {
            if (m_boostAudioKey == AudioCueKey.Invalid) return;
            StopAudio(m_boostAudioKey);
            m_boostAudioKey = AudioCueKey.Invalid;
        }

        public void PlayPreventFallSlowMotion()
        {
            if (m_preventFallSlowMotionKey != AudioCueKey.Invalid) return;
            m_preventFallSlowMotionKey = PlayAudio(_preventFallSlowMotion);
        }

        public void StopPreventFallSlowMotion()
        {
            if (m_preventFallSlowMotionKey == AudioCueKey.Invalid) return;
            StopAudio(m_preventFallSlowMotionKey);
            m_preventFallSlowMotionKey = AudioCueKey.Invalid;
        } 
        
        public void PlayFall()
        {
            if (m_fallKey != AudioCueKey.Invalid) return;
            m_fallKey = PlayAudio(_fall);
        }

        public void StopFall()
        {
            if (m_fallKey == AudioCueKey.Invalid) return;
            StopAudio(m_fallKey);
            m_fallKey = AudioCueKey.Invalid;
        }
        
        public void ChangeMovementVolume(float newVolume)
        {
            if (m_hoverAudioCueKey == AudioCueKey.Invalid) return;
            ChangeAudioClipVolume(m_hoverAudioCueKey, newVolume);
        }

        private void OnDisable()
        {
            StopHoverSound();
        }

        public void PlayTeleportSound() => PlayAudio(_teleport, Vector3.zero);
        public void PlayTeleportImpossibleSound() => PlayAudio(_teleportImpossible, Vector3.zero);
    }
}