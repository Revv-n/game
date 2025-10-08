using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class RectTransformSizeStatable : StatableComponentArray<RectTransform, Vector3>
{
	public override void Set(int stateNumber)
	{
		Vector3 vector = states[stateNumber];
		element.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector.x);
		element.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector.y);
	}
}
