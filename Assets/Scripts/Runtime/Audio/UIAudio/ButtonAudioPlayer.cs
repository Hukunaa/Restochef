using Audio.SFXPlayers;
using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.UIAudio
{
    public class ButtonAudioPlayer : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO buttonSelectedAudioCue;
        [SerializeField] private AudioCueSO buttonClickedAudioCue;

        public void PlayButtonSelectedAudio() => PlayAudio(buttonSelectedAudioCue);
        public void PlayButtonClickedAudio() => PlayAudio(buttonClickedAudioCue);
    }
}
