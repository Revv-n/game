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
		SubscribeDispose = (from sellout in _selloutStateManager.Activated.Where((Sellout sellout) => sellout.ID == _selloutId).Do(delegate(Sellout sellout)
			{
				_sellout = sellout;
			})
			select sellout.Points).Switch().Subscribe(delegate
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
