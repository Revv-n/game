using System;
using GreenT.Types;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public class RelationshipProgressView : MonoView, IDisposable
{
	[SerializeField]
	private CurrencyType _pointsType;

	[SerializeField]
	private RelationshipProgressSlider _pointsSlider;

	private Currencies _currencies;

	private BaseRewardView _rewardView;

	private CompositeIdentificator _id;

	private int _startValue;

	private int _endValue;

	private IDisposable _trackStream;

	private readonly Subject<BaseRewardView> _progressChanged = new Subject<BaseRewardView>();

	public IObservable<bool> IsPressStarted => _pointsSlider.IsPressStarted;

	public IObservable<BaseRewardView> ProgressChanged => Observable.AsObservable<BaseRewardView>((IObservable<BaseRewardView>)_progressChanged);

	[Inject]
	private void Init(Currencies currencies)
	{
		_currencies = currencies;
	}

	public void Set(BaseRewardView rewardView, CompositeIdentificator id, int startValue, int endValue)
	{
		_rewardView = rewardView;
		_id = id;
		_startValue = startValue;
		_endValue = endValue;
		_pointsSlider.Initialization(endValue - startValue);
		StartTrackPoints();
	}

	public int GetStartPoints()
	{
		return _startValue;
	}

	public int GetTargetPoints()
	{
		return _endValue;
	}

	public RectTransform GetHandleRectTransform()
	{
		return _pointsSlider.HandleRectTransform;
	}

	public void Dispose()
	{
		_trackStream?.Dispose();
	}

	private void StartTrackPoints()
	{
		ReactiveProperty<int> val = _currencies.Get(_pointsType, _id);
		SetProgress(val.Value, immediate: true);
		_trackStream = ObservableExtensions.Subscribe<int>((IObservable<int>)val, (Action<int>)delegate(int value)
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
