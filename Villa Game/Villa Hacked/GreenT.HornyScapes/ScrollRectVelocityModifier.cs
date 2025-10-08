using UnityEngine;

namespace GreenT.HornyScapes;

public class ScrollRectVelocityModifier : ScrollRectModifier
{
	[Header("ScrollRect Settings")]
	[SerializeField]
	protected float autoSnappingOffset = 0.02f;

	protected float autoSnappingTopOffset;

	protected virtual void Awake()
	{
		if (axis == RectTransform.Axis.Vertical)
		{
			ScrollRect.onValueChanged.AddListener(AutoVerticalSnapping);
		}
		else
		{
			ScrollRect.onValueChanged.AddListener(AutoHorizontalSnapping);
		}
		autoSnappingTopOffset = 1f - autoSnappingOffset;
	}

	private void AutoVerticalSnapping(Vector2 arg0)
	{
		if (ScrollRect.verticalNormalizedPosition > autoSnappingTopOffset)
		{
			ScrollRect.verticalNormalizedPosition = 1f;
			ScrollRect.velocity = Vector2.zero;
		}
		if (ScrollRect.verticalNormalizedPosition < autoSnappingOffset)
		{
			ScrollRect.verticalNormalizedPosition = 0f;
			ScrollRect.velocity = Vector2.zero;
		}
	}

	private void AutoHorizontalSnapping(Vector2 arg0)
	{
		if (ScrollRect.horizontalNormalizedPosition > autoSnappingTopOffset)
		{
			ScrollRect.horizontalNormalizedPosition = 1f;
			ScrollRect.velocity = Vector2.zero;
		}
		if (ScrollRect.horizontalNormalizedPosition < autoSnappingOffset)
		{
			ScrollRect.horizontalNormalizedPosition = 0f;
			ScrollRect.velocity = Vector2.zero;
		}
	}

	protected virtual void OnDestroy()
	{
		ScrollRect.onValueChanged.RemoveAllListeners();
	}
}
