using System;
using Audio.SFXPlayers;
using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.UIAudio
{
    public class DeathScreenAudioPlayer : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO _deathScreenAudioCue;
        
        private void OnEnable()
        {
            PlayAudio(_deathScreenAudioCue);
        }
    }
}