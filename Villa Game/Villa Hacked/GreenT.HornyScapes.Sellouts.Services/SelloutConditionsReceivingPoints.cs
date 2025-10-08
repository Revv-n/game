using System;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Models;
using UniRx;

namespace GreenT.HornyScapes.Sellouts.Services;

public class SelloutConditionsReceivingPoints : BaseConditionReceivingReward<SelloutConditionsReceivingPoints>
{
	private Sellout _sellout;

	private readonly SelloutStateManager _selloutStateManager;

	private readonly int _selloutId;

	private readonly int _targetValue;

	public override string ConditionText => $"{_targetValue}";

	public SelloutConditionsReceivingPoints(SelloutStateManager selloutStateManager, int selloutId, int targetValue)
	{
		_selloutStateManager = selloutStateManager;
		_selloutId = selloutId;
		_targetValue = targetValue;
	}

	public override bool Validate()
	{
		return ValidateCurrency();
	}

	protected override void Subscribe()
	{
		SubscribeDispose = ObservableExtensions.Subscribe<int>(Observable.Switch<int>((IObservable<IObservable<int>>)Observable.Select<Sellout, IReadOnlyReactiveProperty<int>>(Observable.Do<Sellout>(Observable.Where<Sellout>(_selloutStateManager.Activated, (Func<Sellout, bool>)((Sellout sellout) => sellout.ID == _selloutId)), (Action<Sellout>)delegate(Sellout sellout)
		{
			_sellout = sellout;
		}), (Func<Sellout, IReadOnlyReactiveProperty<int>>)((Sellout sellout) => sellout.Points))), (Action<int>)delegate
		{
			ValidateCurrency();
		});
	}

	protected override bool CheckIfCompleted()
	{
		if (!IsEnoughCurrency())
		{
			return false;
		}
		SetCompleted();
		return true;
	}

	private bool ValidateCurrency()
	{
		if (IsDisabled())
		{
			return false;
		}
		if (IsCompleted())
		{
			return true;
		}
		bool num = IsEnoughCurrency();
		if (num && !IsCompleted())
		{
			SetCompleted();
		}
		return num;
	}

	private bool IsEnoughCurrency()
	{
		if (_sellout != null)
		{
			return _targetValue <= _sellout.Points.Value;
		}
		return false;
	}
}
