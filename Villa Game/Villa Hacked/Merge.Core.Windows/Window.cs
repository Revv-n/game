using System;
using DG.Tweening;
using UnityEngine;

namespace Merge.Core.Windows;

public abstract class Window : MonoBehaviour, IShowCompletableUI, IHideCompletableUI
{
	[SerializeField]
	protected WindowConfig windowConfig;

	public Tween ShowTween { get; private set; }

	public WindowConfig WindowConfig => windowConfig;

	public bool IsPlayingShowTween { get; private set; }

	public bool FullVisible { get; protected set; }

	public event Action<Window> OnBecomeShow;

	public event Action<Window> OnBeckomeHide;

	public event Action<Window> OnEndShow;

	public event Action<Window> OnEndHide;

	public event Action OnShowComplete;

	public event Action OnHideComplete;

	public void Show()
	{
		ShowTween?.Kill(complete: true);
		ShowTween = BuildShowTween();
		Tween showTween = ShowTween;
		showTween.onComplete = (TweenCallback)Delegate.Combine(showTween.onComplete, new TweenCallback(AtEndShow));
		AtBecomeShow();
		this.OnBecomeShow?.Invoke(this);
	}

	public void Hide()
	{
		if (!ShowTween.IsPlaying())
		{
			ShowTween?.Kill(complete: true);
			ShowTween = BuildHideTween();
			Tween showTween = ShowTween;
			showTween.onComplete = (TweenCallback)Delegate.Combine(showTween.onComplete, new TweenCallback(AtEndHide));
			AtBecomeHide();
			this.OnBeckomeHide?.Invoke(this);
		}
	}

	public void TempHide(Action callback)
	{
		ShowTween?.Kill(complete: true);
		ShowTween = BuildHideTween();
		Tween showTween = ShowTween;
		showTween.onComplete = (TweenCallback)Delegate.Combine(showTween.onComplete, (TweenCallback)delegate
		{
			callback?.Invoke();
		});
	}

	public void TempUnhide(Action callback)
	{
		ShowTween?.Kill(complete: true);
		ShowTween = BuildShowTween();
		Tween showTween = ShowTween;
		showTween.onComplete = (TweenCallback)Delegate.Combine(showTween.onComplete, (TweenCallback)delegate
		{
			callback?.Invoke();
		});
	}

	protected virtual void AtBecomeShow()
	{
	}

	protected virtual void AtEndShow()
	{
		FullVisible = true;
		this.OnEndShow?.Invoke(this);
		this.OnShowComplete?.Invoke();
	}

	protected virtual void AtBecomeHide()
	{
		FullVisible = false;
	}

	protected virtual void AtEndHide()
	{
		this.OnEndHide?.Invoke(this);
		this.OnHideComplete?.Invoke();
	}

	protected virtual Tween CreateShowTween()
	{
		Sequence sequence = DOTween.Sequence();
		base.transform.localScale = Vector3.zero;
		base.gameObject.SetActive(value: true);
		CanvasGroup component = GetComponent<CanvasGroup>();
		if (windowConfig.RequersSelfAlpha)
		{
			component.alpha = windowConfig.SelfStartAlpha;
			sequence.Join(DOTweenModuleUI.DOFade(component, 1f, windowConfig.ShowTime));
		}
		sequence.Join(base.transform.DOScale(1f, windowConfig.ShowTime));
		return sequence;
	}

	protected virtual Tween CreateHideTween()
	{
		Sequence sequence = DOTween.Sequence();
		base.transform.localScale = Vector3.one;
		CanvasGroup component = GetComponent<CanvasGroup>();
		if (windowConfig.RequersSelfAlpha)
		{
			component.alpha = 1f;
			sequence.Join(DOTweenModuleUI.DOFade(component, windowConfig.SelfStartAlpha, windowConfig.ShowTime).SetEase(Ease.InSine));
		}
		sequence.Join(base.transform.DOScale(windowConfig.StartScale, windowConfig.ShowTime));
		return sequence;
	}

	private Tween BuildHideTween()
	{
		Tween tween = CreateHideTween();
		IsPlayingShowTween = true;
		tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
		{
			IsPlayingShowTween = false;
			base.gameObject.SetActive(value: false);
		});
		return tween;
	}

	private Tween BuildShowTween()
	{
		base.gameObject.SetActive(value: true);
		Tween tween = CreateShowTween();
		IsPlayingShowTween = true;
		tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
		{
			IsPlayingShowTween = false;
		});
		return tween;
	}

	private void OnDestroy()
	{
		ShowTween?.Kill();
	}
}
