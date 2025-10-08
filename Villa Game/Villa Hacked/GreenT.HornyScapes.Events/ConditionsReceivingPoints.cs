using System;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Events;

public class ConditionsReceivingPoints : BaseConditionReceivingReward<ConditionsReceivingPoints>
{
	private readonly int targetValue;

	private readonly CurrencyType currencyType;

	private readonly ICurrencyProcessor currencyProcessor;

	private readonly CompositeIdentificator _currencyIdentificator;

	public override string ConditionText => "";

	public ConditionsReceivingPoints(ICurrencyProcessor currencyProcessor, CurrencyType currencyType, int targetValue, CompositeIdentificator currencyIdentificator = default(CompositeIdentificator))
	{
		this.currencyProcessor = currencyProcessor;
		this.currencyType = currencyType;
		this.targetValue = targetValue;
		_currencyIdentificator = currencyIdentificator;
	}

	public override bool Validate()
	{
		return ValidateCurrency();
	}

	protected override void Subscribe()
	{
		IReadOnlyReactiveProperty<int> countReactiveProperty = currencyProcessor.GetCountReactiveProperty(currencyType, _currencyIdentificator);
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
		return currencyProcessor.GetCount(currencyType, _currencyIdentificator) >= targetValue;
	}
}
