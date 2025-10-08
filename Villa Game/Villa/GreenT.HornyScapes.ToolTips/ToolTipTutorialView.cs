using DG.Tweening;
using GreenT.HornyScapes.Animations;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipTutorialView : ToolTipView<ToolTipTutorialSettings>
{
	[SerializeField]
	private TypingTextSystem localizedTMPText;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation showAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation hideAnimation;

	private Sequence showSequence;

	public bool Test;

	public override void Set(ToolTipTutorialSettings settings)
	{
		base.Set(settings);
		if (Test)
		{
			localizedTMPText.TestTypingText();
		}
		else
		{
			localizedTMPText.TypingText(settings.KeyText + settings.StepID);
		}
		base.RectTransform.anchoredPosition = settings.ToolTipPosition;
		showAnimation.Init();
		hideAnimation.Init();
	}

	public override void Display(bool display)
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

	private void Show()
	{
		hideAnimation.Stop();
		showSequence = showAnimation.Play();
		base.Display(display: true);
	}

	private void Hide()
	{
		if (showSequence.IsActive())
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
		showAnimation.Stop();
		hideAnimation.Stop();
	}
}
