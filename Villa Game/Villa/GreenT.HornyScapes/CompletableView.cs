using System.Collections.Generic;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public class CompletableView : MonoBehaviour
{
	[SerializeField]
	private List<StatableComponent> statableComponent;

	public void SetComplete(bool state)
	{
		foreach (StatableComponent item in statableComponent)
		{
			item.Set(state ? 1 : 0);
		}
	}
}
