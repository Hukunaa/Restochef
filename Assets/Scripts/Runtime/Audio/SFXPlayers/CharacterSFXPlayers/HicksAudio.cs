using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.SFXPlayers.CharacterSFXPlayers
{
	public class HicksAudio : CharacterAudio
	{
		[SerializeField] private AudioCueSO _footSteps;
		[SerializeField] private AudioCueSO _teleport;
		[SerializeField] private AudioCueSO _teleportImpossible;
		[SerializeField] private AudioCueSO _glide;
		
		private AudioCueKey m_glideAudioCueKey;

		public void PlayFootstep() => PlayAudio(_footSteps, transform.position);
		public void PlayTeleportSound() => PlayAudio(_teleport, transform.position);

		public void PlayTeleportImpossibleSound() => PlayAudio(_teleportImpossible, transform.position);

		private void Awake()
		{
			m_glideAudioCueKey = AudioCueKey.Invalid;
		}

		public void PlayGlideSound()
		{
			if (m_glideAudioCueKey != AudioCueKey.Invalid) return;
			m_glideAudioCueKey = PlayAudio(_glide, transform.position);
		}

		public void StopGlideSound()
		{
			if (m_glideAudioCueKey == AudioCueKey.Invalid) return;
			StopAudio(m_glideAudioCueKey);
			m_glideAudioCueKey = AudioCueKey.Invalid;
		}

		private void OnDisable()
		{
			if (m_glideAudioCueKey != AudioCueKey.Invalid)
			{
				StopGlideSound();
			}
		}
		
		public void ChangeMovementVolume(float newVolume)
		{
			if (m_glideAudioCueKey == AudioCueKey.Invalid) return;
			ChangeAudioClipVolume(m_glideAudioCueKey, newVolume);
		}
	}
}
