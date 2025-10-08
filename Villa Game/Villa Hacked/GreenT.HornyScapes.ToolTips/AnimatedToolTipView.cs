using DG.Tweening;
using GreenT.HornyScapes.Animations;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public abstract class AnimatedToolTipView<T> : ToolTipView<T> where T : ToolTipSettings
{
	[SerializeField]
	protected Animation showAnimation;

	[SerializeField]
	protected Animation hideAnimation;

	private Sequence showSequence;

	public override void Set(T settings)
	{
		showAnimation.Stop();
		hideAnimation.Stop();
		base.Set(settings);
		showAnimation.Init();
		hideAnimation.Init();
	}

	public override void Display(bool display)
	{
		if (IsActive() != display)
		{
			if (display)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}

	private void Show()
	{
		hideAnimation.Stop();
		showSequence = showAnimation.Play();
		base.Display(display: true);
	}

	private void Hide()
	{
		if (showSequence.IsActive() && showSequence.IsPlaying())
		{
			showSequence.OnComplete(delegate
			{
				hideAnimation.Play().OnComplete(delegate
				{
					base.Display(display: false);
				});
			});
		}
		else
		{
			hideAnimation.Play().OnComplete(delegate
			{
				base.Display(display: false);
			});
		}
	}

	protected virtual void OnDisable()
	{
		showAnimation.ResetToAnimStart();
		hideAnimation.ResetToAnimStart();
		base.Display(display: false);
	}
}
