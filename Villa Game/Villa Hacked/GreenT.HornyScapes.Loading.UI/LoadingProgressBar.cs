using DG.Tweening;
using GreenT.HornyScapes.UI;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Loading.UI;

public sealed class LoadingProgressBar : MonoBehaviour, ILoadingScreen
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	[Space]
	[SerializeField]
	private float fadeDuration = 0.5f;

	[SerializeField]
	private int waitInSecondOnCompleteBeforeHide = 1;

	[SerializeField]
	private FloatClampedProgress progressBar;

	[SerializeField]
	private GameObject _localizationButton;

	public bool isFade;

	private Tweener fadeTweener;

	private Sequence progressAnimationSequence;

	private DiContainer container;

	[Inject]
	public void Init(DiContainer container)
	{
		this.container = container;
	}

	public void Initialize(bool fadeOnChangeState)
	{
		StopAnimation();
		progressBar.Init(0f);
		isFade = fadeOnChangeState;
	}

	public void SetProgress(float progress, float animationTimeInSecs = 1f)
	{
		StopAnimation();
		Tweener t = DOTween.To(() => progressBar.Progress, progressBar.Init, progress, animationTimeInSecs).OnStart(delegate
		{
			DisplayLoadingStatus(display: true);
		});
		progressAnimationSequence = DOTween.Sequence().Join(t);
		if (progress == 1f)
		{
			progressAnimationSequence = progressAnimationSequence.AppendInterval(waitInSecondOnCompleteBeforeHide).OnComplete(delegate
			{
				DisplayLoadingStatus(display: false);
			});
		}
	}

	public void SetOnConfigsLoaded()
	{
		_localizationButton.SetActive(value: true);
	}

	private void DisplayLoadingStatus(bool display)
	{
		progressBar.gameObject.SetActive(display);
	}

	private void StopAnimation()
	{
		if (progressAnimationSequence.IsActive())
		{
			progressAnimationSequence.Kill();
		}
	}

	public void SetLoadingScreenActive(bool isActive)
	{
		fadeTweener?.Complete(withCallbacks: true);
		if (isActive)
		{
			fadeTweener = DOTweenModuleUI.DOFade(canvasGroup, 1f, isFade ? fadeDuration : 0f).OnStart(delegate
			{
				canvasGroup.alpha = 0f;
			});
			return;
		}
		fadeTweener = DOTweenModuleUI.DOFade(canvasGroup, 0f, isFade ? fadeDuration : 0f).OnStart(delegate
		{
			canvasGroup.alpha = 1f;
		}).OnComplete(delegate
		{
			Object.Destroy(base.gameObject);
		});
	}

	private void OnDestroy()
	{
		container.Unbind(typeof(LoadingProgressBar));
		progressAnimationSequence.Complete(withCallbacks: true);
		fadeTweener?.Complete(withCallbacks: true);
	}
}
