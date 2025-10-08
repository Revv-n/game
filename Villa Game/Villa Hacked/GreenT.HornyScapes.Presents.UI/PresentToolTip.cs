using DG.Tweening;
using GreenT.HornyScapes.ToolTips;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Presents.UI;

public class PresentToolTip : AnimatedToolTipView<TailedToolTipSettings>
{
	internal class Manager : MonoViewManager<TailedToolTipSettings, PresentToolTip>
	{
	}

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private float _tooltipDuration;

	private Sequence _showSequence;

	private Tween _hideDelayTween;

	public float TooltipDuration => _tooltipDuration;

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

	private void Awake()
	{
		showAnimation.Init();
		hideAnimation.Init();
	}

	private void Show()
	{
		_hideDelayTween?.Kill();
		if (base.Source != null)
		{
			base.transform.localPosition = base.Source.ToolTipPosition;
			_text.text = base.Source.KeyText;
		}
		hideAnimation.Stop();
		_showSequence = showAnimation.Play();
		base.Display(display: true);
		_hideDelayTween = DOVirtual.DelayedCall(_tooltipDuration, Hide);
	}

	private void Hide()
	{
		if (_showSequence != null && _showSequence.IsActive() && _showSequence.IsPlaying())
		{
			_showSequence.OnComplete(delegate
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
