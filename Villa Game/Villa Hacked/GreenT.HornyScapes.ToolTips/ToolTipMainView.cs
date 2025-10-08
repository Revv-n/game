using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipMainView : ToolTipView<ToolTipSettings>
{
	internal class Manager : MonoViewManager<ToolTipSettings, ToolTipMainView>
	{
	}

	[SerializeField]
	private AbstractZoomScaler zoomScaler;

	[SerializeField]
	private LocalizedTextMeshPro localizedText;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private Animation showAnimation;

	[SerializeField]
	private Animation hideAnimation;

	private Sequence showSequence;

	private void Awake()
	{
		zoomScaler.StartScaling();
	}

	public override void Set(ToolTipSettings settings)
	{
		base.Set(settings);
		if (settings != null)
		{
			base.transform.localPosition = settings.ToolTipPosition;
			localizedText.Init(settings.KeyText);
		}
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
