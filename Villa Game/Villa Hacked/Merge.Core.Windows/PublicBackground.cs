using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Merge.Core.Windows;

public class PublicBackground : MonoBehaviour
{
	[SerializeField]
	private Image back;

	[SerializeField]
	private EventTrigger trigger;

	[SerializeField]
	private float fadeTime = 0.6f;

	[SerializeField]
	private Ease ease;

	private Tween fadeTween;

	public bool AllowCloseClick { get; set; } = true;


	public float TargetAlpha { get; private set; }

	public event Action OnClick;

	private void Start()
	{
		trigger.AddClickCallback(AtClick);
	}

	public PublicBackground DoAlpha(float alpha, bool fromZero = true)
	{
		base.gameObject.SetActive(value: true);
		fadeTween?.Kill();
		if (fromZero)
		{
			back.SetAlpha(0f);
		}
		TargetAlpha = alpha;
		fadeTween = DOTweenModuleUI.DOFade(back, TargetAlpha, fadeTime).SetEase(ease);
		return this;
	}

	public PublicBackground DoHide(float time = 0f)
	{
		fadeTween?.Kill();
		fadeTween = DOTweenModuleUI.DOFade(back, 0f, (time == 0f) ? fadeTime : time).SetEase(ease);
		fadeTween.OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
		return this;
	}

	private void AtClick()
	{
		if (AllowCloseClick)
		{
			this.OnClick();
		}
	}

	private void OnDestroy()
	{
		fadeTween?.Kill();
	}
}
