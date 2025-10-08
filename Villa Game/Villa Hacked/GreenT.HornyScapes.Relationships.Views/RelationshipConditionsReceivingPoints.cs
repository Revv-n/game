using System;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Relationships.Views;

public class RelationshipConditionsReceivingPoints : BaseConditionReceivingReward<RelationshipConditionsReceivingPoints>
{
	private readonly int _targetValue;

	private readonly CurrencyType _currencyType;

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly CompositeIdentificator _currencyIdentificator;

	public override string ConditionText => $"{_targetValue}";

	public RelationshipConditionsReceivingPoints(ICurrencyProcessor currencyProcessor, CurrencyType currencyType, int targetValue, CompositeIdentificator currencyIdentificator = default(CompositeIdentificator))
	{
		_currencyProcessor = currencyProcessor;
		_currencyType = currencyType;
		_targetValue = targetValue;
		_currencyIdentificator = currencyIdentificator;
	}

	public override bool Validate()
	{
		return ValidateCurrency();
	}

	protected override void Subscribe()
	{
		IReadOnlyReactiveProperty<int> countReactiveProperty = _currencyProcessor.GetCountReactiveProperty(_currencyType, _currencyIdentificator);
		SubscribeDispose = ObservableExtensions.Subscribe<int>((IObservable<int>)countReactiveProperty, (Action<int>)delegate
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
		return _currencyProcessor.GetCount(_currencyType, _currencyIdentificator) >= _targetValue;
	}
}
