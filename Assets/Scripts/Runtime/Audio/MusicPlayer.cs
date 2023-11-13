using GeneralScriptableObjects.SceneData;
using Runtime.Audio.AudioData;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;

namespace Audio
{
	public class MusicPlayer : MonoBehaviour
	{
		[SerializeField] private VoidEventChannel _onSceneReady = default;
		[SerializeField] private AudioCueEventChannel _playMusicOn = default;
		[SerializeField] private GameSceneSO _thisSceneSO = default;
		[SerializeField] private AudioConfigurationSO _audioConfig = default;

		private void OnEnable()
		{
			//_onPauseOpened.OnEventRaised += PlayPauseMusic;
			_onSceneReady.onEventRaised += PlayMusic;
		}

		private void OnDisable()
		{
			_onSceneReady.onEventRaised -= PlayMusic;
			//_onPauseOpened.OnEventRaised -= PlayPauseMusic;
		}

		private void PlayMusic()
		{
			_playMusicOn.RaisePlayEvent(_thisSceneSO.musicTrack, _audioConfig);
		}

		public void StopMusic()
		{
			_playMusicOn.RaiseStopEvent(AudioCueKey.Invalid);
		}

		public void FadeMusicOut(float fadeDuration)
		{
			_playMusicOn.RaiseFadeOutEvent(AudioCueKey.Invalid, fadeDuration);
		}
	}
}
