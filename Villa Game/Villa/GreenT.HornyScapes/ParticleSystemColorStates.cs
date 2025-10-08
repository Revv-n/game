using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public class ParticleSystemColorStates : StatableComponent
{
	[SerializeField]
	private ParticleSystem _particle;

	[SerializeField]
	private IntColorDictionary _colors = new IntColorDictionary();

	private void OnValidate()
	{
		if (_particle == null)
		{
			_particle = GetComponent<ParticleSystem>();
		}
	}

	public override void Set(int stateNumber)
	{
		ParticleSystem.MainModule main = _particle.main;
		main.startColor = _colors[stateNumber];
	}
}
