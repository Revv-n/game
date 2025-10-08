using System;
using GreenT.HornyScapes.Analytics;
using UniRx;

namespace GreenT.HornyScapes;

public interface ICurrenciesActionContainer : IDisposable
{
	int Count { get; }

	IReadOnlyReactiveProperty<int> ReactiveCount { get; }

	CurrencyAmplitudeAnalytic.SourceType LastSourceType { get; }

	bool TryAdd(int value, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None);

	bool TrySpend(int value);

	bool IsEnough(int value);

	IObservable<int> OnSpend();

	IObservable<int> OnAdd();

	void Reset();
}
