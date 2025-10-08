using System.Collections.Generic;
using System.Linq;
using GreenT.Bonus;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Booster.Effect;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class EnergyCurrencyView : CurrencyView
{
	[SerializeField]
	private float _sizeModifier;

	[SerializeField]
	private Transform _iconTransform;

	[SerializeField]
	private StatableComponentGroup _capBonusStatable;

	[SerializeField]
	private StatableComponentGroup _rechargeBonusStatable;

	private BoosterStorage _boosterStorage;

	[Inject]
	public void Init(BoosterStorage boosterStorage)
	{
		_boosterStorage = boosterStorage;
	}

	protected override void SetupView()
	{
		SetupBonusHandle();
		currencySpriteAttacher.SetView();
		value.text = current.Value.ToString();
		updateValueStream?.Dispose();
		updateValueStream = current.Pairwise().SelectMany((Pair<int> values) => SmoothAdd(values.Previous, values.Current)).Subscribe(OnValueChange, delegate
		{
			Set(current.Value);
		}, delegate
		{
			Set(current.Value);
		});
	}

	private void SetupBonusHandle()
	{
		_boosterStorage.Collection.ObserveEveryValueChanged((IEnumerable<BoosterModel> collection) => collection.Count()).Subscribe(delegate
		{
			HandleCapBonus();
			HandleRechargeBonus();
		}).AddTo(this);
	}

	private void HandleCapBonus()
	{
		bool flag = IsBonusActive(BonusType.IncreaseBaseEnergy);
		float num = (flag ? _sizeModifier : 1f);
		_iconTransform.localScale = Vector3.one * num;
		_capBonusStatable.Set(flag ? 1 : 0);
	}

	private void HandleRechargeBonus()
	{
		bool flag = IsBonusActive(BonusType.IncreaseEnergyRechargeSpeed);
		_rechargeBonusStatable.Set(flag ? 1 : 0);
	}

	private bool IsBonusActive(BonusType bonusType)
	{
		return _boosterStorage.Collection.Any((BoosterModel model) => model.Bonus is BoosterIncrementBonus boosterIncrementBonus && boosterIncrementBonus.BonusType == bonusType);
	}

	private void OnValueChange(int _newValue)
	{
		value.text = _newValue.ToString();
	}
}
