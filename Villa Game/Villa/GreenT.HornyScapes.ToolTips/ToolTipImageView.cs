using DG.Tweening;
using GreenT.HornyScapes.Animations;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipImageView : ToolTipView<ToolTipImageSettings>
{
	internal class Manager : MonoViewManager<ToolTipImageSettings, ToolTipImageView>
	{
	}

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private LocalizedTextMeshPro localizedTMPText;

	[SerializeField]
	private RectTransform tailBody;

	[SerializeField]
	private RectTransform tailHead;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation showAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation hideAnimation;

	private Sequence showSequence;

	public override void Set(ToolTipImageSettings settings)
	{
		base.Set(settings);
		localizedTMPText.Init(settings.KeyText);
		base.RectTransform.anchoredPosition = settings.ToolTipPosition;
		base.RectTransform.pivot = settings.PivotPosition;
		InitTail(settings.TailSettings);
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

	private void InitTail(Tail settings)
	{
		RectTransform obj = ((settings.TailType == TailType.Body) ? tailBody : tailHead);
		obj.gameObject.SetActive(value: true);
		obj.anchoredPosition = settings.TailPosition;
		obj.localRotation = Quaternion.Euler(settings.TailRotation);
	}
}
