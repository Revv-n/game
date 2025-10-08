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

	public IObservable<PresentView> PresentSended => _presentSended.AsObservable();

	public IObservable<PresentView> PressStarted => _pressStarted.AsObservable();

	public IObservable<PresentView> PressEnded => _pressEnded.AsObservable();

	public void Track(IEnumerable<PresentView> presentsViews)
	{
		_disposables?.Clear();
		foreach (PresentView presentView in presentsViews)
		{
			presentView.PointerDown.Do(delegate
			{
				_pressStarted?.OnNext(presentView);
				TrySendPresent(presentView);
			}).SelectMany((PointerEventData _) => CreatePresentSendStream(presentView)).Subscribe(delegate
			{
				TrySendPresent(presentView);
			})
				.AddTo(_disposables);
			presentView.PointerUp.Subscribe(delegate
			{
				_pressEnded?.OnNext(presentView);
			}).AddTo(_disposables);
		}
	}

	private IObservable<Unit> CreatePresentSendStream(PresentView presentView)
	{
		float startTime = Time.time;
		float nextGiftTime = startTime + GetInterval(presentView, 0f);
		return from _ in Observable.EveryUpdate().TakeUntil(presentView.PointerUp)
			where ShouldSendNextPresent(presentView, ref nextGiftTime, startTime)
			select Unit.Default;
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
		_disposables?.Clear();
	}
}
