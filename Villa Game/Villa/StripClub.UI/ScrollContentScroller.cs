using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI;

public class ScrollContentScroller : MonoBehaviour
{
	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private float duration;

	[SerializeField]
	private Ease ease;

	public void ScrollToIndex(int i)
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(content);
		float num = 0f;
		for (int j = 0; j < i; j++)
		{
			float width = (content.GetChild(j).transform as RectTransform).rect.width;
			num += width;
		}
		num = Mathf.Clamp(num, 0f, content.rect.width);
		content.DOAnchorPosX(0f - num, duration).SetEase(ease);
	}
}
