using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipGirlView : ToolTipView<ToolTipSettings>
{
	internal class Manager : MonoViewManager<ToolTipSettings, ToolTipGirlView>
	{
	}

	[SerializeField]
	private AbstractZoomScaler zoomScaler;

	[SerializeField]
	private LocalizedTextMeshPro localizedText;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation showAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation hideAnimation;

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
			string text = settings.KeyText;
			if (text[text.Length - 1] == '.')
			{
				text += Random.Range(1, 11);
			}
			localizedText.Init(text);
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
		if (showSequence != null && showSequence.IsActive() && showSequence.IsPlaying())
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
