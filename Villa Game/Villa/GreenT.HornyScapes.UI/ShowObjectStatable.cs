using System.Collections.Generic;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class ShowObjectStatable : StatableComponent
{
	[SerializeField]
	private List<bool> states;

	public override void Set(int stateNumber)
	{
		base.gameObject.SetActive(states[stateNumber]);
	}
}
