using DG.Tweening;
using GreenT.HornyScapes.Animations;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class HighlightedTextView : ToolTipView<ToolTipSettings>
{
	internal class Manager : MonoViewManager<ToolTipSettings, HighlightedTextView>
	{
	}

	[SerializeField]
	private LocalizedTextMeshPro localizedText;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private Animation showAnimation;

	[SerializeField]
	private Animation hideAnimation;

	private Sequence showSequence;

	[SerializeField]
	private Canvas canvas;

	public override void Set(ToolTipSettings settings)
	{
		base.Set(settings);
		localizedText.Init(settings.KeyText);
		canvas.transform.localPosition = Vector3.zero;
		canvas.transform.localScale = Vector3.one;
		showAnimation.Init();
		hideAnimation.Init();
	}

	protected override void SetParent(ToolTipSettings source)
	{
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

	private void OnDisable()
	{
		showAnimation.ResetToAnimStart();
		base.Display(display: false);
	}

	private void Hide()
	{
		if (showSequence != null && showSequence.IsPlaying())
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
}
