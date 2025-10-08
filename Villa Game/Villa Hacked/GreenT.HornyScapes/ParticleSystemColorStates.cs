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
		if ((Object)(object)_particle == null)
		{
			_particle = GetComponent<ParticleSystem>();
		}
	}

	public override void Set(int stateNumber)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		MainModule main = _particle.main;
		((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(_colors[stateNumber]);
	}
}
