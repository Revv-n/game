using System;
using GreenT.HornyScapes.BattlePassSpace;
using UniRx;

namespace GreenT.HornyScapes;

public class BattlePassLevelProvider : IDisposable
{
	private readonly ReactiveProperty<int> battlePassCurrency;

	private IDisposable _battlePassLevelStream;

	private IDisposable _battleChangeStream;

	public IReadOnlyReactiveProperty<int> Level => (IReadOnlyReactiveProperty<int>)(object)battlePassCurrency;

	public BattlePassLevelProvider(BattlePassProvider battlePassProvider)
	{
		battlePassCurrency = new ReactiveProperty<int>();
		_battleChangeStream = ObservableExtensions.Subscribe<BattlePass>(Observable.Select<(CalendarModel, BattlePass), BattlePass>((IObservable<(CalendarModel, BattlePass)>)battlePassProvider.CalendarChangeProperty, (Func<(CalendarModel, BattlePass), BattlePass>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass)), (Action<BattlePass>)SubscribeNewBattlePassData);
	}

	private void SubscribeNewBattlePassData(BattlePass battlePass)
	{
		if (battlePass?.Data?.LevelInfo != null)
		{
			IReadOnlyReactiveProperty<int> points = battlePass.Data.LevelInfo.Points;
			_battlePassLevelStream = ObservableExtensions.Subscribe<int>((IObservable<int>)points, (Action<int>)OnChangeValue);
		}
		else
		{
			battlePassCurrency.Value = 0;
		}
	}

	private void OnChangeValue(int i)
	{
		battlePassCurrency.Value = i;
	}

	public void Dispose()
	{
		_battlePassLevelStream?.Dispose();
		_battleChangeStream?.Dispose();
	}
}
