using System;
using GreenT.HornyScapes.Sellouts.Models;
using StripClub.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Sellouts.Views;

public class SelloutProgressContainer : MonoView, IDisposable
{
	[SerializeField]
	private SelloutProgressSlider _pointsSlider;

	private SelloutRewardView _rewardView;

	private int _startValue;

	private int _endValue;

	private IDisposable _trackStream;

	private readonly Subject<SelloutRewardView> _progressChanged = new Subject<SelloutRewardView>();

	public IObservable<SelloutRewardView> ProgressChanged => Observable.AsObservable<SelloutRewardView>((IObservable<SelloutRewardView>)_progressChanged);

	public void Set(Sellout sellout, SelloutRewardView rewardView, int startValue, int endValue)
	{
		_rewardView = rewardView;
		_startValue = startValue;
		_endValue = endValue;
		_pointsSlider.Initialization(endValue - startValue);
		StartTrackPoints(sellout);
	}

	public void Dispose()
	{
		_trackStream?.Dispose();
	}

	private void StartTrackPoints(Sellout sellout)
	{
		IReadOnlyReactiveProperty<int> points = sellout.Points;
		SetProgress(points.Value, immediate: true);
		_trackStream = ObservableExtensions.Subscribe<int>((IObservable<int>)points, (Action<int>)delegate(int value)
		{
			SetProgress(value);
		});
	}

	private void SetProgress(int value, bool immediate = false)
	{
		if (value < _startValue)
		{
			_pointsSlider.SetProgress(0, 0f, immediate);
			return;
		}
		int current = value - _startValue;
		_pointsSlider.SetProgress(current, 0f, immediate);
		if (_startValue <= value && value <= _endValue)
		{
			_progressChanged?.OnNext(_rewardView);
		}
	}
}
