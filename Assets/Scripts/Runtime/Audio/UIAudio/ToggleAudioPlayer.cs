using Audio.SFXPlayers;
using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.UIAudio
{
    public class ToggleAudioPlayer : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO toggledAudioCue;
        [SerializeField] private AudioCueSO toggleSelectedAudioCue;

        public void PlayToggleAudio() => PlayAudio(toggledAudioCue);
        public void PlayToggleSelectedAudio() => PlayAudio(toggleSelectedAudioCue);
    }
}