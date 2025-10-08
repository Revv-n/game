using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class ParallaxScrollEffect : MonoView
{
	[SerializeField]
	private ScrollRect _scrollRect;

	[SerializeField]
	private RectTransform _background;

	[SerializeField]
	private float _parallaxFactor = 0.5f;

	private float _initialBackgroundPosX;

	private float _viewportWidth;

	private float _backgroundWidth;

	private void Awake()
	{
		_initialBackgroundPosX = _background.anchoredPosition.x;
		_viewportWidth = _scrollRect.viewport.rect.width;
		_backgroundWidth = _background.rect.width;
	}

	private void OnEnable()
	{
		_scrollRect.onValueChanged.AddListener(OnScroll);
		UpdateBackgroundPosition(_scrollRect.normalizedPosition);
	}

	private void OnDisable()
	{
		_scrollRect.onValueChanged.RemoveListener(OnScroll);
	}

	private void OnScroll(Vector2 normalizedPosition)
	{
		UpdateBackgroundPosition(normalizedPosition);
	}

	private void UpdateBackgroundPosition(Vector2 normalizedPosition)
	{
		float num = (_backgroundWidth - _viewportWidth) * _parallaxFactor;
		float num2 = (_backgroundWidth - _viewportWidth) * 0.5f - num * normalizedPosition.x;
		_background.anchoredPosition = new Vector2(_initialBackgroundPosX + num2, _background.anchoredPosition.y);
	}
}
