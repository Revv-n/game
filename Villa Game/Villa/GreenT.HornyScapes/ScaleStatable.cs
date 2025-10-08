using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public class ScaleStatable : StatableComponent
{
	[SerializeField]
	private Transform _target;

	[SerializeField]
	private Vector3[] _settings;

	public override void Set(int stateNumber)
	{
		_target.transform.localScale = _settings[stateNumber];
	}
}
