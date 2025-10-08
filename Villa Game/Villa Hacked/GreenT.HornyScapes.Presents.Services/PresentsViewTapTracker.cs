using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Presents.UI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GreenT.HornyScapes.Presents.Services;

public class PresentsViewTapTracker
{
	private readonly Subject<PresentView> _presentSended = new Subject<PresentView>();

	private readonly Subject<PresentView> _pressStarted = new Subject<PresentView>();

	private readonly Subject<PresentView> _pressEnded = new Subject<PresentView>();

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public IObservable<PresentView> PresentSended => Observable.AsObservable<PresentView>((IObservable<PresentView>)_presentSended);

	public IObservable<PresentView> PressStarted => Observable.AsObservable<PresentView>((IObservable<PresentView>)_pressStarted);

	public IObservable<PresentView> PressEnded => Observable.AsObservable<PresentView>((IObservable<PresentView>)_pressEnded);

	public void Track(IEnumerable<PresentView> presentsViews)
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Clear();
		}
		foreach (PresentView presentView in presentsViews)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.SelectMany<PointerEventData, Unit>(Observable.Do<PointerEventData>(presentView.PointerDown, (Action<PointerEventData>)delegate
			{
				_pressStarted?.OnNext(presentView);
				TrySendPresent(presentView);
			}), (Func<PointerEventData, IObservable<Unit>>)((PointerEventData _) => CreatePresentSendStream(presentView))), (Action<Unit>)delegate
			{
				TrySendPresent(presentView);
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PointerEventData>(presentView.PointerUp, (Action<PointerEventData>)delegate
			{
				_pressEnded?.OnNext(presentView);
			}), (ICollection<IDisposable>)_disposables);
		}
	}

	private IObservable<Unit> CreatePresentSendStream(PresentView presentView)
	{
		float startTime = Time.time;
		float nextGiftTime = startTime + GetInterval(presentView, 0f);
		return Observable.Select<long, Unit>(Observable.Where<long>(Observable.TakeUntil<long, PointerEventData>(Observable.EveryUpdate(), presentView.PointerUp), (Func<long, bool>)((long _) => ShouldSendNextPresent(presentView, ref nextGiftTime, startTime))), (Func<long, Unit>)((long _) => Unit.Default));
	}

	private bool ShouldSendNextPresent(PresentView presentView, ref float nextGiftTime, float startTime)
	{
		float time = Time.time;
		if (time < nextGiftTime)
		{
			return false;
		}
		float elapsed = time - startTime;
		nextGiftTime = time + GetInterval(presentView, elapsed);
		return true;
	}

	private void TrySendPresent(PresentView presentView)
	{
		_presentSended.OnNext(presentView);
	}

	private float GetInterval(PresentView presentView, float elapsed)
	{
		Present source = presentView.Source;
		float[] longtapStageTime = source.LongtapStageTime;
		float[] longtapSpeed = source.LongtapSpeed;
		for (int num = longtapStageTime.Length - 1; num >= 0; num--)
		{
			if (elapsed >= longtapStageTime[num])
			{
				return longtapSpeed[num];
			}
		}
		if (longtapSpeed.Length == 0)
		{
			return 1f;
		}
		return longtapSpeed[0];
	}

	public void Untrack()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Clear();
		}
	}
}
