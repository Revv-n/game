using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Windows;

public sealed class RelationshipBackground : MonoBehaviour
{
	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private SpriteStates _spriteStates;

	private float _width;

	private float _previousWidth;

	public float Width => _width;

	public float PreviousWidth => _previousWidth;

	public Vector2 Position => _rectTransform.anchoredPosition;

	public void SetScale(Vector3 scale)
	{
		_rectTransform.localScale = scale;
	}

	public void AddWidth(float width)
	{
		_previousWidth = _width;
		_width += width;
		_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _width);
	}

	public void SetPosition(in Vector2 position)
	{
		_rectTransform.anchoredPosition = position;
	}

	public void SetStatus(int status)
	{
		_spriteStates.Set(status);
	}

	public void Clear()
	{
		_width = 0f;
		_previousWidth = 0f;
		_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _width);
		_rectTransform.anchoredPosition = Vector2.zero;
	}
}
