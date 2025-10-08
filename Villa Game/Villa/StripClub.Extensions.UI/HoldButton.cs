using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StripClub.Extensions.UI;

[AddComponentMenu("UI/Hold Button")]
public class HoldButton : Button
{
	private PointerEventData d;

	[Range(0f, 10f)]
	public float holdTimeTrigger = 1f;

	[Range(0f, 10f)]
	public float waitBeforeDetectHold = 2f;

	[Tooltip("The base of increase power. Greater value - greater step ")]
	public float increaseSpeed = 1f;

	[Tooltip("Frequency of changing step size")]
	public int increaseFrequency = 1;

	private int holdLenght;

	public event Action<int> onContinueHold;

	protected override void Start()
	{
		base.Start();
		this.OnPointerUpAsObservable().Subscribe(delegate
		{
			if (holdLenght == 0)
			{
				this.onContinueHold?.Invoke(1);
			}
		}).AddTo(this);
		this.OnPointerDownAsObservable().Subscribe(delegate
		{
			holdLenght = 0;
		});
		this.OnPointerDownAsObservable().SelectMany(this.UpdateAsObservable()).Delay(TimeSpan.FromSeconds(waitBeforeDetectHold))
			.ThrottleFirst(TimeSpan.FromSeconds(holdTimeTrigger))
			.TakeUntil(this.OnPointerUpAsObservable())
			.RepeatUntilDestroy(this)
			.Subscribe(delegate
			{
				Holding();
			});
	}

	public void Holding()
	{
		holdLenght++;
		int obj = (int)Mathf.Pow(increaseSpeed, holdLenght / increaseFrequency);
		this.onContinueHold?.Invoke(obj);
	}
}
