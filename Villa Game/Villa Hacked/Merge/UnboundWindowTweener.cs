using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class UnboundWindowTweener : MonoBehaviour
{
	[SerializeField]
	private List<UnboundWindowPopItem> popItems;

	[SerializeField]
	private TransformPointsPath titlePP;

	[SerializeField]
	private float time;

	[SerializeField]
	private AnimationCurve popCurve;

	[SerializeField]
	private Image back;

	[SerializeField]
	private float backFade = 0.7f;

	private Tween showTween;

	public Tween DoShowTween(List<UnboundWindowPopItem> items, float contentOffset = 0f, bool titleAndBack = true)
	{
		return DoTween(direct: true, items, contentOffset, titleAndBack);
	}

	public Tween DoShowTween(float contentOffset = 0f)
	{
		return DoTween(direct: true, contentOffset);
	}

	public Tween DoHideTween(List<UnboundWindowPopItem> items, bool titleAndBack = true)
	{
		return DoTween(direct: false, items, 0f, titleAndBack);
	}

	public Tween DoHideTween()
	{
		return DoTween(direct: false);
	}

	private Tween DoTween(bool direct, float contentOffset = 0f)
	{
		return DoTween(direct, popItems, contentOffset);
	}

	private Tween DoTween(bool direct, List<UnboundWindowPopItem> items, float contentOffset = 0f, bool titleAndBack = true)
	{
		if (titleAndBack)
		{
			if (direct)
			{
				titlePP.ToStart();
			}
			else
			{
				titlePP.ToEnd();
			}
		}
		showTween?.Kill();
		Sequence sequence = DOTween.Sequence();
		if (titleAndBack)
		{
			sequence.Join(titlePP.DoWalk(time, direct));
			sequence.Join(DOTweenModuleUI.DOFade(back, direct ? backFade : 0f, time + ((items.Count <= 1) ? 0f : items[0].SizeTime)).SetEase(Ease.InSine));
		}
		if (items.Count == 1)
		{
			sequence.Join(items[0].DoPop(direct));
		}
		else if (items.Count > 1)
		{
			float num = Mathf.Max(items.Max((UnboundWindowPopItem x) => x.Priority), 1);
			foreach (UnboundWindowPopItem item in items)
			{
				if (!item.IsIgnored)
				{
					float num2 = popCurve.Evaluate((float)item.Priority / num) * time;
					if (!direct)
					{
						num2 = time - num2;
					}
					if (items.Count == 1)
					{
						num2 = 0f;
					}
					Tween t = item.DoPop(direct);
					sequence.Insert(num2, t);
				}
			}
		}
		showTween = sequence;
		return sequence;
	}
}
