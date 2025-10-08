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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PointerEventData>(ObservableTriggerExtensions.OnPointerUpAsObservable((UIBehaviour)this), (Action<PointerEventData>)delegate
		{
			if (holdLenght == 0)
			{
				this.onContinueHold?.Invoke(1);
			}
		}), (Component)this);
		ObservableExtensions.Subscribe<PointerEventData>(ObservableTriggerExtensions.OnPointerDownAsObservable((UIBehaviour)this), (Action<PointerEventData>)delegate
		{
			holdLenght = 0;
		});
		ObservableExtensions.Subscribe<Unit>(Observable.RepeatUntilDestroy<Unit>(Observable.TakeUntil<Unit, PointerEventData>(Observable.ThrottleFirst<Unit>(Observable.Delay<Unit>(Observable.SelectMany<PointerEventData, Unit>(ObservableTriggerExtensions.OnPointerDownAsObservable((UIBehaviour)this), ObservableTriggerExtensions.UpdateAsObservable((Component)this)), TimeSpan.FromSeconds(waitBeforeDetectHold)), TimeSpan.FromSeconds(holdTimeTrigger)), ObservableTriggerExtensions.OnPointerUpAsObservable((UIBehaviour)this)), (Component)this), (Action<Unit>)delegate
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
