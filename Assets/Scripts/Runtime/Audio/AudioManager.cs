using System.Collections;
using Audio.SoundEmitters;
using DG.Tweening;
using GeneralScriptableObjects;
using Runtime.Audio.AudioData;
using Runtime.Audio.SoundEmitters;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
	public class AudioManager : MonoBehaviour
	{
		[Header("SoundEmitters pool")]
		[SerializeField] private SoundEmitterPoolSO _pool;
		[SerializeField] private int _initialSize = 10;

		[Header("Listening on channels")]
		[Tooltip("The SoundManager listens to this event, fired by objects in any scene, to play SFXs")]
		[SerializeField] private AudioCueEventChannel _SFXEventChannel;
		[Tooltip("The SoundManager listens to this event, fired by objects in any scene, to play Music")]
		[SerializeField] private AudioCueEventChannel _musicEventChannel;
		[Tooltip("The SoundManager listens to this event, fired by objects in any scene, to change SFXs volume")]
		[SerializeField] private FloatEventChannel _SFXVolumeEventChannel;
		[Tooltip("The SoundManager listens to this event, fired by objects in any scene, to change Music volume")]
		[SerializeField] private FloatEventChannel _musicVolumeEventChannel;
		[Tooltip("The SoundManager listens to this event, fired by objects in any scene, to change Master volume")]
		[SerializeField] private FloatEventChannel _masterVolumeEventChannel;
		
		[Header("Audio control")]
		[SerializeField] private AudioMixer audioMixer;
		[SerializeField] private FloatVariable _masterVolume;
		[SerializeField] private FloatVariable _musicVolume;
		[SerializeField] private FloatVariable _sfxVolume;
		
		private SoundEmitterVault _soundEmitterVault;
		private SoundEmitter _musicSoundEmitter;

		private AudioCueSO _currentAudioCue;
		private AudioConfigurationSO _currentMusicAudioConfiguration;
		private int _audioClipIndex;
		
		private void Awake()
		{
			//TODO: Get the initial volume levels from the settings
			_soundEmitterVault = new SoundEmitterVault();
		
			_pool.Prewarm(_initialSize);
			_pool.SetParent(this.transform);
			
			ChangeMasterVolume(_masterVolume.Value);
			ChangeMusicVolume(_musicVolume.Value);
			ChangeSFXVolume(_sfxVolume.Value);
		}

		private void OnEnable()
		{
			_SFXEventChannel.onAudioCuePlayRequested += PlayAudioCue;
			_SFXEventChannel.onAudioCueStopRequested += StopAudioCue;
			_SFXEventChannel.onAudioCueFinishRequested += FinishAudioCue;
			_SFXEventChannel.onChangeVolumeRequested += ChangeVFXVolume;

			_musicEventChannel.onAudioCuePlayRequested += PlayMusicTrack;
			_musicEventChannel.onAudioCueStopRequested += StopMusic;
			_musicEventChannel.onAudioCueFadeOutRequested += FadeMusicTrackOut;

			_masterVolumeEventChannel.onEventRaised += ChangeMasterVolume;
			_musicVolumeEventChannel.onEventRaised += ChangeMusicVolume;
			_SFXVolumeEventChannel.onEventRaised += ChangeSFXVolume;
		}

		private void OnDestroy()
		{
			_SFXEventChannel.onAudioCuePlayRequested -= PlayAudioCue;
			_SFXEventChannel.onAudioCueStopRequested -= StopAudioCue;
			_SFXEventChannel.onAudioCueFinishRequested -= FinishAudioCue;
			_SFXEventChannel.onChangeVolumeRequested -= ChangeVFXVolume;
			
			_musicEventChannel.onAudioCuePlayRequested -= PlayMusicTrack;
			_musicEventChannel.onAudioCueStopRequested -= StopMusic;
			_musicEventChannel.onAudioCueFadeOutRequested -= FadeMusicTrackOut;
			
			_masterVolumeEventChannel.onEventRaised -= ChangeMasterVolume;
			_musicVolumeEventChannel.onEventRaised -= ChangeMusicVolume;
			_SFXVolumeEventChannel.onEventRaised -= ChangeSFXVolume;
		}

		/// <summary>
		/// This is only used in the Editor, to debug volumes.
		/// It is called when any of the variables is changed, and will directly change the value of the volumes on the AudioMixer.
		/// </summary>

		void ChangeMasterVolume(float newVolume)
		{
			SetGroupVolume("MasterVolume", newVolume);
		}
		
		void ChangeMusicVolume(float newVolume)
		{
			SetGroupVolume("MusicVolume", newVolume);
		}
		
		void ChangeSFXVolume(float newVolume)
		{
			SetGroupVolume("SFXVolume", newVolume);
		}
		
		public void SetGroupVolume(string parameterName, float normalizedVolume)
		{
			audioMixer.DOSetFloat(parameterName, NormalizedToMixerValue(normalizedVolume), 1);
		}

		public float GetGroupVolume(string parameterName)
		{
			if (audioMixer.GetFloat(parameterName, out float rawVolume))
			{
				return MixerValueToNormalized(rawVolume);
			}
			else
			{
				Debug.LogError("The AudioMixer parameter was not found");
				return 0f;
			}
		}

		// Both MixerValueNormalized and NormalizedToMixerValue functions are used for easier transformations
		/// when using UI sliders normalized format
		private float MixerValueToNormalized(float mixerValue)
		{
			// We're assuming the range [-80dB to 0dB] becomes [0 to 1]
			return 1f + (mixerValue / 80f);
		}
		private float NormalizedToMixerValue(float normalizedValue)
		{
			// We're assuming the range [0 to 1] becomes [-80dB to 0dB]
			// This doesn't allow values over 0dB
			return (normalizedValue - 1f) * 80f;
		}

		private bool FadeMusicTrackOut(AudioCueKey key, float fadeDuration)
		{
			if (_musicSoundEmitter == null || !_musicSoundEmitter.IsPlaying()) return false;
			
			_musicSoundEmitter.FadeMusicOut(fadeDuration);
			_musicSoundEmitter.OnSoundFinishedPlaying -= PlayNextMusicClip;
			StartCoroutine(StopSoundEmitterWithDelay(_musicSoundEmitter, fadeDuration));
			return true;
		}

		private AudioCueKey PlayMusicTrack(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace)
		{
			if (_currentAudioCue != null && audioCue == _currentAudioCue) return AudioCueKey.Invalid;

			if(audioCue._resetPlayingOrder) audioCue.ResetAudioClipsGroupPlayingOrder();
		
			_currentAudioCue = audioCue;
			_currentMusicAudioConfiguration = audioConfiguration;
			_audioClipIndex = 0;
		
			float fadeDuration = 2f;

			if (_musicSoundEmitter != null && _musicSoundEmitter.IsPlaying())
			{
				AudioClip songToPlay = audioCue.GetClips()[_audioClipIndex];
				if (_musicSoundEmitter.GetClip() == songToPlay)
					return AudioCueKey.Invalid;

				//Music is already playing, need to fade it out
				_musicSoundEmitter.FadeMusicOut(fadeDuration);
				_musicSoundEmitter.OnSoundFinishedPlaying -= PlayNextMusicClip;
				StartCoroutine(StopSoundEmitterWithDelay(_musicSoundEmitter, fadeDuration));
			}

			_musicSoundEmitter = _pool.Request();
			_musicSoundEmitter.FadeMusicIn(audioCue.GetClips()[0], audioConfiguration, 1f);
			_musicSoundEmitter.OnSoundFinishedPlaying += PlayNextMusicClip;

			return AudioCueKey.Invalid; //No need to return a valid key for music
		}

		private IEnumerator StopSoundEmitterWithDelay(SoundEmitter soundEmitter, float delay)
		{
			yield return new WaitForSeconds(delay);
		
			StopAndCleanEmitter(soundEmitter);
		}

		private void PlayNextMusicClip(SoundEmitter soundEmitter)
		{
			AudioClip[] clips = _currentAudioCue.GetClips();
			_audioClipIndex++;
			if (_audioClipIndex == clips.Length)
			{
				_audioClipIndex = 0;
			}
			_musicSoundEmitter.FadeMusicIn(clips[_audioClipIndex], _currentMusicAudioConfiguration, 1f);
		}

		private bool StopMusic(AudioCueKey key)
		{
			if (_musicSoundEmitter == null || !_musicSoundEmitter.IsPlaying()) return false;
			
			_musicSoundEmitter.Stop();
			return true;
		}

		/// <summary>
		/// Only used by the timeline to stop the gameplay music during cutscenes.
		/// Called by the SignalReceiver present on this same GameObject.
		/// </summary>
		public void TimelineInterruptsMusic()
		{
			StopMusic(AudioCueKey.Invalid);
		}

		/// <summary>
		/// Plays an AudioCue by requesting the appropriate number of SoundEmitters from the pool.
		/// </summary>
		public AudioCueKey PlayAudioCue(AudioCueSO audioCue, AudioConfigurationSO settings, Vector3 position = default)
		{
			AudioClip[] clipsToPlay = audioCue.GetClips();
			SoundEmitter[] soundEmitterArray = new SoundEmitter[clipsToPlay.Length];

			int nOfClips = clipsToPlay.Length;
			for (int i = 0; i < nOfClips; i++)
			{
				soundEmitterArray[i] = _pool.Request();
				if (soundEmitterArray[i] != null)
				{
					soundEmitterArray[i].PlayAudioClip(clipsToPlay[i], settings, audioCue.looping, position);
					if (!audioCue.looping)
						soundEmitterArray[i].OnSoundFinishedPlaying += OnSoundEmitterFinishedPlaying;
				}
			}

			return _soundEmitterVault.Add(audioCue, soundEmitterArray);
		}

		public bool FinishAudioCue(AudioCueKey audioCueKey)
		{
			bool isFound = _soundEmitterVault.Get(audioCueKey, out SoundEmitter[] soundEmitters);

			if (isFound)
			{
				for (int i = 0; i < soundEmitters.Length; i++)
				{
					soundEmitters[i].Finish();
					soundEmitters[i].OnSoundFinishedPlaying += OnSoundEmitterFinishedPlaying;
				}
			}
			else
			{
				Debug.LogWarning("Finishing an AudioCue was requested, but the AudioCue was not found.");
			}

			return isFound;
		}

		public bool StopAudioCue(AudioCueKey audioCueKey)
		{
			bool isFound = _soundEmitterVault.Get(audioCueKey, out SoundEmitter[] soundEmitters);

			if (isFound)
			{
				for (int i = 0; i < soundEmitters.Length; i++)
				{
					StopAndCleanEmitter(soundEmitters[i]);
				}

				_soundEmitterVault.Remove(audioCueKey);
			}

			return isFound;
		}

		public void ChangeVFXVolume(AudioCueKey audioCueKey, float volume)
		{
			bool isFound = _soundEmitterVault.Get(audioCueKey, out SoundEmitter[] soundEmitters);

			if (isFound)
			{
				for (int i = 0; i < soundEmitters.Length; i++)
				{
					soundEmitters[i].ChangeVolume(volume);
				}
			}
			else
			{
				Debug.LogWarning("Finishing an AudioCue was requested, but the AudioCue was not found.");
			}
		}

		private void OnSoundEmitterFinishedPlaying(SoundEmitter soundEmitter)
		{
			StopAndCleanEmitter(soundEmitter);
		}

		private void StopAndCleanEmitter(SoundEmitter soundEmitter)
		{
			if (!soundEmitter.IsLooping())
				soundEmitter.OnSoundFinishedPlaying -= OnSoundEmitterFinishedPlaying;

			soundEmitter.Stop();
			_pool.Return(soundEmitter);

			//TODO: is the above enough?
			//_soundEmitterVault.Remove(audioCueKey); is never called if StopAndClean is called after a Finish event
			//How is the key removed from the vault?
		}

		private void StopMusicEmitter(SoundEmitter soundEmitter)
		{
			soundEmitter.OnSoundFinishedPlaying -= StopMusicEmitter;
			_pool.Return(soundEmitter);
		}
	}
}
