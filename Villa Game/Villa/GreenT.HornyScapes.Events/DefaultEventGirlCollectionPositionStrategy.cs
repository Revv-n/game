using UnityEngine;

namespace GreenT.HornyScapes.Events;

public sealed class DefaultEventGirlCollectionPositionStrategy : IEventGirlCollectionPositionStrategy
{
	private readonly RectTransform _rectTransform;

	private readonly Vector2 _defaultPosition;

	public DefaultEventGirlCollectionPositionStrategy(RectTransform rectTransform)
	{
		_rectTransform = rectTransform;
		_defaultPosition = _rectTransform.anchoredPosition;
	}

	public void UpdatePosition()
	{
		_rectTransform.anchoredPosition = _defaultPosition;
	}
}
