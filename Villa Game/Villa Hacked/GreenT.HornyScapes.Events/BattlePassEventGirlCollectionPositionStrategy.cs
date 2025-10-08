using UnityEngine;

namespace GreenT.HornyScapes.Events;

public sealed class BattlePassEventGirlCollectionPositionStrategy : IEventGirlCollectionPositionStrategy
{
	private readonly RectTransform _rectTransform;

	private readonly Vector2 _defaultPosition;

	public BattlePassEventGirlCollectionPositionStrategy(RectTransform rectTransform)
	{
		_rectTransform = rectTransform;
		Vector2 vector = new Vector2(0f, -40f);
		_defaultPosition = _rectTransform.anchoredPosition + vector;
	}

	public void UpdatePosition()
	{
		_rectTransform.anchoredPosition = _defaultPosition;
	}
}
