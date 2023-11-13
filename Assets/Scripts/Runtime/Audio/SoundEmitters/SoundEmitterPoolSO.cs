using Audio.SoundEmitters;
using Factory;
using Pool;
using UnityEngine;

namespace Runtime.Audio.SoundEmitters
{
	[CreateAssetMenu(fileName = "NewSoundEmitterPool", menuName = "ScriptableObjects/Pool/SoundEmitter Pool")]
	public class SoundEmitterPoolSO : ComponentPoolSO<SoundEmitter>
	{
		[SerializeField] private SoundEmitterFactorySO _factory;

		public override IFactory<SoundEmitter> Factory
		{
			get
			{
				return _factory;
			}
			set
			{
				_factory = value as SoundEmitterFactorySO;
			}
		}
	}
}
