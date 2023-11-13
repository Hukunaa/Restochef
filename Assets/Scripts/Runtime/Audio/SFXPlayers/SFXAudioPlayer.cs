using Runtime.Audio.AudioData;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;

namespace Audio.SFXPlayers
{
    public abstract class SFXAudioPlayer : MonoBehaviour
    {
        [SerializeField] protected AudioCueEventChannel _sfxEventChannel;
        [SerializeField] protected AudioConfigurationSO _audioConfig;

        protected AudioCueKey PlayAudio(AudioCueSO audioCue, Vector3 positionInSpace = default)
        {
            return _sfxEventChannel.RaisePlayEvent(audioCue, _audioConfig, positionInSpace);
        }

        protected void StopAudio(AudioCueKey audioCueKey)
        {
            _sfxEventChannel.RaiseStopEvent(audioCueKey);
        }

        protected void ChangeAudioClipVolume(AudioCueKey key, float newVolume)
        {
            _sfxEventChannel.RaiseChangeVolumeEvent(key, newVolume);
        }
    }
}