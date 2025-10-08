using DG.Tweening;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class BankInfoToolTip : AnimatedToolTipView<TailedToolTipSettings>
{
	internal class Manager : MonoViewManager<TailedToolTipSettings, BankInfoToolTip>
	{
	}

	[SerializeField]
	private LocalizedTextMeshPro localizedText;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private RectTransform _tail;

	private Sequence showSequence;

	public override void Set(TailedToolTipSettings settings)
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
		if (base.Source.TailSettings.TailPosition != Vector3.zero)
		{
			Vector2 anchoredPosition = new Vector2(base.Source.TailSettings.TailPosition.x, _tail.anchoredPosition.y);
			_tail.anchoredPosition = anchoredPosition;
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
