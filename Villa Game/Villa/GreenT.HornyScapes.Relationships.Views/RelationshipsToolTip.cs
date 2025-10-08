using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;
using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public class RelationshipsToolTip : MonoView<TailedToolTipSettings>
{
	internal class Manager : MonoViewManager<TailedToolTipSettings, RelationshipsToolTip>
	{
	}

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation _showAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation _hideAnimation;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private float _tooltipDuration;

	private bool _isInitialized;

	private Sequence _showSequence;

	private Tween _hideDelayTween;

	public override void Set(TailedToolTipSettings settings)
	{
		base.Set(settings);
		SetParent();
		_rectTransform.anchoredPosition = base.Source.ToolTipPosition;
		_rectTransform.pivot = base.Source.PivotPosition;
		InitAnimation();
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

	public void ForceHide()
	{
		_showAnimation?.Stop();
		_hideAnimation?.Stop();
		_showAnimation = null;
		_hideAnimation = null;
	}

	private void InitAnimation()
	{
		if (!_isInitialized)
		{
			_showAnimation.Init();
			_hideAnimation.Init();
			_isInitialized = true;
		}
	}

	private void SetParent()
	{
		base.transform.SetParent(base.Source.Parent, worldPositionStays: false);
	}

	private void Show()
	{
		_hideDelayTween?.Kill();
		if (base.Source != null)
		{
			base.transform.localPosition = base.Source.ToolTipPosition;
			_text.text = base.Source.KeyText;
		}
		_hideAnimation.Stop();
		_showSequence = _showAnimation.Play();
		base.Display(display: true);
		_hideDelayTween = DOVirtual.DelayedCall(_tooltipDuration, Hide);
	}

	private void Hide()
	{
		if (_showSequence != null && _showSequence.IsActive() && _showSequence.IsPlaying())
		{
			_showSequence.OnComplete(delegate
			{
				_hideAnimation.Play().OnComplete(delegate
				{
					base.Display(display: false);
				});
			});
		}
		else
		{
			_hideAnimation.Play().OnComplete(delegate
			{
				base.Display(display: false);
			});
		}
	}
}
