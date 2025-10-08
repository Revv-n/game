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
		_actionContainer = new BattlePassCurrenciesActionContainer((IReadOnlyReactiveProperty<int>)(object)_battlePassCurrency, null, mainBalance);
		_battlePassProvider = battlePassProvider;
	}

	public void Initialize()
	{
		_battlePassChangeStream = ObservableExtensions.Subscribe<BattlePass>(Observable.Select<(CalendarModel, BattlePass), BattlePass>((IObservable<(CalendarModel, BattlePass)>)_battlePassProvider.CalendarChangeProperty, (Func<(CalendarModel, BattlePass), BattlePass>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass)), (Action<BattlePass>)SubscribeNewBattlePassData);
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
			_currencyChangeStream = ObservableExtensions.Subscribe<int>((IObservable<int>)points, (Action<int>)OnChangeValue);
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
