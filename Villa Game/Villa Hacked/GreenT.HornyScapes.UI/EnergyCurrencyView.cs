using System;
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
		updateValueStream = ObservableExtensions.Subscribe<int>(Observable.SelectMany<Pair<int>, int>(Observable.Pairwise<int>((IObservable<int>)current), (Func<Pair<int>, IObservable<int>>)((Pair<int> values) => SmoothAdd(values.Previous, values.Current))), (Action<int>)OnValueChange, (Action<Exception>)delegate
		{
			Set(current.Value);
		}, (Action)delegate
		{
			Set(current.Value);
		});
	}

	private void SetupBonusHandle()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(ObserveExtensions.ObserveEveryValueChanged<IEnumerable<BoosterModel>, int>(_boosterStorage.Collection, (Func<IEnumerable<BoosterModel>, int>)((IEnumerable<BoosterModel> collection) => collection.Count()), (FrameCountType)0, false), (Action<int>)delegate
		{
			HandleCapBonus();
			HandleRechargeBonus();
		}), (Component)this);
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
