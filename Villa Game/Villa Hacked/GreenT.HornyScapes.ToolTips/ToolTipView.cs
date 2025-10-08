using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public abstract class ToolTipView<T> : MonoView<T> where T : ToolTipSettings
{
	public RectTransform RectTransform { get; private set; }

	public override void Set(T source)
	{
		base.Set(source);
		SetParent(source);
		RectTransform.anchoredPosition = source.ToolTipPosition;
		RectTransform.pivot = source.PivotPosition;
	}

	protected virtual void SetParent(T source)
	{
		base.transform.SetParent(source.Parent, worldPositionStays: false);
	}
}
