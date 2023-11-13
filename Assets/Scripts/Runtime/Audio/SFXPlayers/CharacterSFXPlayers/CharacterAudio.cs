using Runtime.Audio.AudioData;
using UnityEngine;

namespace Audio.SFXPlayers.CharacterSFXPlayers
{
	public class CharacterAudio : SFXAudioPlayer
	{
		[SerializeField] private AudioCueSO _interaction;
		[SerializeField] private AudioCueSO _interactionFailed;
		[SerializeField] private AudioCueSO _deathAudio;
		
		public void PlayInteractionSound(bool interactionSucceeded)
		{
			PlayAudio(interactionSucceeded ? _interaction : _interactionFailed);
		}
		
		public void PlayDeathSound()
		{
			PlayAudio(_deathAudio);
		}
	}
}
