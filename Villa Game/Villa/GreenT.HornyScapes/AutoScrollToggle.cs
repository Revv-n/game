using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class AutoScrollToggle : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _text;

	[SerializeField]
	private ScrollRect _scrollRect;

	[SerializeField]
	private RectTransform _contentRect;

	public void Set()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(_text.rectTransform);
		float preferredHeight = _text.preferredHeight;
		float height = _scrollRect.viewport.rect.height;
		Vector2 sizeDelta = _contentRect.sizeDelta;
		sizeDelta.y = preferredHeight;
		_contentRect.sizeDelta = sizeDelta;
		_scrollRect.vertical = preferredHeight > height;
		_contentRect.anchoredPosition *= Vector2.right;
	}
}
