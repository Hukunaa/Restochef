using UnityEngine;

namespace Audio
{
	public class PlayAudioOnAnimationEvent : MonoBehaviour
	{
		public AudioSource audioClip;

		public void PlayClip()
		{
			audioClip.Play();
		}
   
	}
}
