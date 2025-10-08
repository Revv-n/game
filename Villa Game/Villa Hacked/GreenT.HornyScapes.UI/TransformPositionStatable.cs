using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class TransformPositionStatable : StatableComponentArray<RectTransform, Vector3>
{
	public override void Set(int stateNumber)
	{
		element.anchoredPosition = states[stateNumber];
	}
}
