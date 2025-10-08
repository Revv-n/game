using DG.Tweening;
using UnityEngine;

namespace Merge;

public class UIHider : UIHiderBase
{
	[SerializeField]
	private CanvasGroup group;

	[SerializeField]
	private float time = 0.3f;

	private Tween visibleTween;

	public override bool IsVisible { get; protected set; }

	public override void DoVisible(bool visible)
	{
		visibleTween?.Kill();
		IsVisible = visible;
		group.interactable = visible;
		visibleTween = group.DOFade(visible ? 1 : 0, time);
	}

	public override void SetVisible(bool visible)
	{
		visibleTween?.Kill();
		IsVisible = visible;
		group.alpha = (visible ? 1 : 0);
	}
}
