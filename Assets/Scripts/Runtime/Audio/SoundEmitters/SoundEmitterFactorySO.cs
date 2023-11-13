using Audio.SoundEmitters;
using Factory;
using UnityEngine;

namespace Runtime.Audio.SoundEmitters
{
	[CreateAssetMenu(fileName = "NewSoundEmitterFactory", menuName = "ScriptableObjects/Factory/SoundEmitter Factory")]
	public class SoundEmitterFactorySO : FactorySO<SoundEmitter>
	{
		public SoundEmitter prefab = default;

		public override SoundEmitter Create()
		{
			return Instantiate(prefab);
		}
	}
}
