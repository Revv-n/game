using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class ItemClock : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image fill;

	[SerializeField]
	private Transform root;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Sprite mixing;

	[SerializeField]
	private Sprite energy;

	[SerializeField]
	private Sprite locker;

	[Header("Animation")]
	[SerializeField]
	private float startScale = 0.7f;

	[SerializeField]
	private float startAlpha = 0.7f;

	[SerializeField]
	private float doScaleDuration = 0.1f;

	[SerializeField]
	private float doPunchDuration = 1f;

	[SerializeField]
	private float punchScale = 0.05f;

	private Tween activeTimer;

	private bool setToState;

	public Transform Root => root;

	public TimerStatus ActualTimerInfo { get; private set; }

	public void SetEnergySprite()
	{
		icon.sprite = energy;
	}

	public void SetLockerSprite()
	{
		icon.sprite = locker;
	}

	public void SetMixingSprite()
	{
		icon.sprite = mixing;
	}

	public ItemClock SetActive(bool active)
	{
		if (setToState == active)
		{
			return this;
		}
		setToState = active;
		activeTimer?.Kill(complete: true);
		activeTimer = (active ? ShowTimer() : HideTimer());
		return this;
	}

	public ItemClock SetTimerInfo(TimerStatus timerInfo)
	{
		ActualTimerInfo = timerInfo;
		fill.fillAmount = timerInfo.Percent;
		return this;
	}

	private Tween ShowTimer()
	{
		base.transform.localScale = Vector3.one * startScale;
		canvasGroup.alpha = startAlpha;
		base.gameObject.SetActive(value: true);
		return DOTween.Sequence().Join(canvasGroup.DOFade(1f, doScaleDuration)).Append(base.transform.DOScale(Vector3.one, doScaleDuration))
			.Append(base.transform.DOPunchScale(Vector3.one * punchScale, doPunchDuration, 1));
	}

	private Tween HideTimer()
	{
		return DOTween.Sequence().Append(base.transform.DOPunchScale(Vector3.one * punchScale, doPunchDuration, 1)).Insert(doPunchDuration, canvasGroup.DOFade(startAlpha, doScaleDuration))
			.Append(base.transform.DOScale(Vector3.one * startScale, doScaleDuration))
			.OnComplete(delegate
			{
				base.gameObject.SetActive(value: false);
			});
	}
}
