using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.BattlePassSpace.Data;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes;

public class BattlePassCurrenciesActionContainer : ICurrenciesActionContainer, IDisposable
{
	private BattlePasLevelInfoCase levelInfoCase;

	private readonly Subject<int> addStream;

	private readonly Currencies mainBalance;

	public int Count => ReactiveCount.Value;

	public IReadOnlyReactiveProperty<int> ReactiveCount { get; }

	public CurrencyAmplitudeAnalytic.SourceType LastSourceType { get; private set; }

	public IObservable<int> OnSpend()
	{
		return null;
	}

	public IObservable<int> OnAdd()
	{
		return addStream;
	}

	public BattlePassCurrenciesActionContainer(IReadOnlyReactiveProperty<int> reactiveCount, BattlePasLevelInfoCase levelInfoCase, Currencies mainBalance)
	{
		ReactiveCount = reactiveCount;
		this.levelInfoCase = levelInfoCase;
		this.mainBalance = mainBalance;
		addStream = new Subject<int>();
	}

	public void UpdateLevelInfo(BattlePasLevelInfoCase levelInfoCase)
	{
		this.levelInfoCase = levelInfoCase;
	}

	public bool TryAdd(int value, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None)
	{
		if (value < 0 || levelInfoCase == null)
		{
			return false;
		}
		mainBalance.OldBattlePass.Count.Value += value;
		levelInfoCase.AddPoints(value);
		addStream.OnNext(value);
		return true;
	}

	public bool TrySpend(int value)
	{
		return false;
	}

	public bool IsEnough(int value)
	{
		return true;
	}

	public void Reset()
	{
	}

	public void Dispose()
	{
		addStream.Dispose();
	}
}
