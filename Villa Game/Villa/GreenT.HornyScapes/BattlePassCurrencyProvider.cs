using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.BattlePassSpace.Data;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public class BattlePassCurrencyProvider : IDisposable
{
	private readonly BattlePassProvider _battlePassProvider;

	private readonly ReactiveProperty<int> _battlePassCurrency;

	private readonly BattlePassCurrenciesActionContainer _actionContainer;

	private IDisposable _battlePassChangeStream;

	private IDisposable _currencyChangeStream;

	public BattlePassCurrencyProvider(BattlePassProvider battlePassProvider, Currencies mainBalance)
	{
		_battlePassCurrency = new ReactiveProperty<int>();
		_actionContainer = new BattlePassCurrenciesActionContainer(_battlePassCurrency, null, mainBalance);
		_battlePassProvider = battlePassProvider;
	}

	public void Initialize()
	{
		_battlePassChangeStream = _battlePassProvider.CalendarChangeProperty.Select(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass).Subscribe(SubscribeNewBattlePassData);
	}

	public bool TryGetContainer(out ICurrenciesActionContainer container)
	{
		container = _actionContainer;
		return true;
	}

	private void SubscribeNewBattlePassData(BattlePass battlePass)
	{
		if (battlePass?.Data?.LevelInfo != null)
		{
			BattlePasLevelInfoCase levelInfo = battlePass.Data.LevelInfo;
			IReadOnlyReactiveProperty<int> points = levelInfo.Points;
			_currencyChangeStream = points.Subscribe(OnChangeValue);
			_battlePassCurrency.Value = points.Value;
			_actionContainer.UpdateLevelInfo(levelInfo);
		}
		else
		{
			_battlePassCurrency.Value = 0;
		}
	}

	private void OnChangeValue(int i)
	{
		_battlePassCurrency.Value = i;
	}

	public void Dispose()
	{
		_battlePassChangeStream?.Dispose();
		_currencyChangeStream?.Dispose();
	}
}
