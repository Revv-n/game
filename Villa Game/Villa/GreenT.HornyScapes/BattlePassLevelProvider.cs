using System;
using GreenT.HornyScapes.BattlePassSpace;
using UniRx;

namespace GreenT.HornyScapes;

public class BattlePassLevelProvider : IDisposable
{
	private readonly ReactiveProperty<int> battlePassCurrency;

	private IDisposable _battlePassLevelStream;

	private IDisposable _battleChangeStream;

	public IReadOnlyReactiveProperty<int> Level => battlePassCurrency;

	public BattlePassLevelProvider(BattlePassProvider battlePassProvider)
	{
		battlePassCurrency = new ReactiveProperty<int>();
		_battleChangeStream = battlePassProvider.CalendarChangeProperty.Select(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass).Subscribe(SubscribeNewBattlePassData);
	}

	private void SubscribeNewBattlePassData(BattlePass battlePass)
	{
		if (battlePass?.Data?.LevelInfo != null)
		{
			IReadOnlyReactiveProperty<int> points = battlePass.Data.LevelInfo.Points;
			_battlePassLevelStream = points.Subscribe(OnChangeValue);
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
