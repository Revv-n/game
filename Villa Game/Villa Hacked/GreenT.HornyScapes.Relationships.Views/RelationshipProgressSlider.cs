using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Presents.Services;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class RelationshipProgressSlider : ProgressSlider
{
	private const float FillDuration = 0.2f;

	[SerializeField]
	private GameObject _handle;

	[SerializeField]
	private RectTransform _handleRectTransform;

	private PresentsViewTapTracker _presentsViewTapTracker;

	private int _target;

	private readonly Subject<bool> _isPressStarted = new Subject<bool>();

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public RectTransform HandleRectTransform => _handleRectTransform;

	public IObservable<bool> IsPressStarted => (IObservable<bool>)_isPressStarted;

	[Inject]
	public void Init(PresentsViewTapTracker presentsViewTapTracker)
	{
		_presentsViewTapTracker = presentsViewTapTracker;
	}

	public void Initialization(int target)
	{
		_target = target;
		if (_handle != null)
		{
			_handle.SetActive(value: false);
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PresentView>(_presentsViewTapTracker.PressStarted, (Action<PresentView>)delegate
		{
			_isPressStarted.OnNext(true);
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PresentView>(Observable.Where<PresentView>(_presentsViewTapTracker.PressEnded, (Func<PresentView, bool>)((PresentView _) => base.Slider.minValue < base.Slider.value && base.Slider.value < base.Slider.maxValue)), (Action<PresentView>)delegate
		{
			_isPressStarted.OnNext(false);
		}), (ICollection<IDisposable>)_disposables);
		Reset();
	}

	public void SetProgress(int current, float min = 0f, bool immediate = false)
	{
		SetProgress(current, _target, min, immediate);
	}

	public override void Init(float relativeProgress)
	{
		DOVirtual.Float(base.Slider.value, Clamp(relativeProgress), 0.2f, delegate(float x)
		{
			base.Slider.value = x;
		}).OnComplete(delegate
		{
			if (_handle != null)
			{
				DisableHandleOnFill(relativeProgress);
			}
		});
	}

	public void Reset()
	{
		base.Slider.value = 0f;
	}

	private void OnDestroy()
	{
		_disposables.Dispose();
	}

	private void Fill(float relativeProgress, bool immediate)
	{
		if (immediate)
		{
			base.Init(relativeProgress);
			if (_handle != null)
			{
				DisableHandleOnFill(relativeProgress);
			}
		}
		else
		{
			Init(relativeProgress);
		}
	}

	private void DisableHandleOnFill(float relativeProgress)
	{
		if (relativeProgress >= 1f)
		{
			_handle.SetActive(value: false);
		}
	}

	private void SetProgress(float value, float max, float min = 0f, bool immediate = false)
	{
		if (!(max < 0f))
		{
			if (_handle != null)
			{
				_handle.SetActive(value != 0f && base.Slider.value != 1f);
			}
			if (min > max)
			{
				Fill(1f, immediate);
			}
			else if (max == 0f)
			{
				Fill(1f, immediate);
			}
			else
			{
				Fill((value - min) / (max - min), immediate);
			}
		}
	}
}
