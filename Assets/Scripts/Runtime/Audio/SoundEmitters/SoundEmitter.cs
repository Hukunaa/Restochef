using System.Collections;
using DG.Tweening;
using Runtime.Audio.AudioData;
using UnityEngine;
using UnityEngine.Events;

namespace Audio.SoundEmitters
{
	[RequireComponent(typeof(AudioSource))]
	public class SoundEmitter : MonoBehaviour
	{
		private AudioSource m_audioSource;

		public event UnityAction<SoundEmitter> OnSoundFinishedPlaying;
	
		private Transform m_followedTransform;

		private void Awake()
		{
			m_audioSource = this.GetComponent<AudioSource>();
			m_audioSource.playOnAwake = false;
		}

		private void FixedUpdate()
		{
			if (m_followedTransform)
			{
				m_audioSource.transform.position = m_followedTransform.position;
			}
		}

		/// <summary>
		/// Instructs the AudioSource to play a single clip, with optional looping, in a position in 3D space.
		/// </summary>
		/// <param name="clip"></param>
		/// <param name="settings"></param>
		/// <param name="hasToLoop"></param>
		/// <param name="position"></param>
		public void PlayAudioClip(AudioClip clip, AudioConfigurationSO settings, bool hasToLoop, Vector3 position = default)
		{
			m_audioSource.clip = clip;
			settings.ApplyTo(m_audioSource);
			m_audioSource.transform.position = position;
			m_audioSource.loop = hasToLoop;
			m_audioSource.time = 0f; //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
			m_audioSource.Play();

			if (!hasToLoop)
			{
				StartCoroutine(FinishedPlaying(clip.length));
			}
		}
	
		public void PlayAudioClip(AudioClip clip, AudioConfigurationSO settings, bool hasToLoop, Transform followedTransform)
		{
			m_followedTransform = followedTransform;
			m_audioSource.clip = clip;
			settings.ApplyTo(m_audioSource);
			m_audioSource.transform.position = followedTransform.position;
			m_audioSource.loop = hasToLoop;
			m_audioSource.time = 0f; //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
			m_audioSource.Play();

			if (!hasToLoop)
			{
				StartCoroutine(FinishedPlaying(clip.length));
			}
		}

		public void ChangeVolume(float volume)
		{
			m_audioSource.volume = volume;
		}

		public void FadeMusicIn(AudioClip musicClip, AudioConfigurationSO settings, float duration)
		{
			PlayAudioClip(musicClip, settings, false);
			m_audioSource.volume = 0f;
			m_audioSource.time = 0f;

			m_audioSource.DOFade(settings.Volume, duration);
		}

		public float FadeMusicOut(float duration)
		{
			m_audioSource.DOFade(0f, duration).onComplete += OnFadeOutComplete;

			return m_audioSource.time;
		}

		private void OnFadeOutComplete()
		{
			NotifyBeingDone();
		}

		/// <summary>
		/// Used to check which music track is being played.
		/// </summary>
		public AudioClip GetClip()
		{
			return m_audioSource.clip;
		}
	
		/// <summary>
		/// Used when the game is unpaused, to pick up SFX from where they left.
		/// </summary>
		public void Resume()
		{
			m_audioSource.Play();
		}

		/// <summary>
		/// Used when the game is paused.
		/// </summary>
		public void Pause()
		{
			m_audioSource.Pause();
		}

		public void Stop()
		{
			m_audioSource.Stop();
		}

		public void Finish()
		{
			if (m_audioSource.loop)
			{
				m_audioSource.loop = false;
				float timeRemaining = m_audioSource.clip.length - m_audioSource.time;
				StartCoroutine(FinishedPlaying(timeRemaining));
			}
		}

		public bool IsPlaying()
		{
			return m_audioSource.isPlaying;
		}

		public bool IsLooping()
		{
			return m_audioSource.loop;
		}

		IEnumerator FinishedPlaying(float clipLength)
		{
			yield return new WaitForSeconds(clipLength);

			NotifyBeingDone();
		}

		private void NotifyBeingDone()
		{
			OnSoundFinishedPlaying?.Invoke(this); // The AudioManager will pick this up
		}
	}
}
