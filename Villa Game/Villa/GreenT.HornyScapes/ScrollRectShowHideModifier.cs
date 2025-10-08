using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class ScrollRectShowHideModifier : ScrollRectModifier
{
	private float contentTop;

	private float contentBottom;

	protected void CalcContentBounds(ScrollRect givenScroll)
	{
		contentTop = (1f - givenScroll.verticalNormalizedPosition) * (givenScroll.content.rect.height - givenScroll.viewport.rect.height);
		contentBottom = contentTop + givenScroll.viewport.rect.height;
	}

	public bool HideObject(CanvasGroup canvasGroup, float givenPosition, float givenHeight)
	{
		float num = Mathf.Abs(givenPosition);
		float num2 = num + givenHeight;
		if ((num2 > contentTop && num2 < contentBottom) || (num > contentTop && num < contentBottom))
		{
			if (canvasGroup.alpha != 1f)
			{
				canvasGroup.alpha = 1f;
			}
			return true;
		}
		if (canvasGroup.alpha != 0f)
		{
			canvasGroup.alpha = 0f;
		}
		return false;
	}
}
