using System;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model.Data;
using UniRx;

namespace StripClub.Model.Shop;

public interface ICurrencyProcessor
{
	bool IsEnough(Cost cost, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	bool IsEnough(CurrencyType type, int amount, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	bool TrySpent(Cost cost, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	bool TrySpent(CurrencyType type, int value, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	bool TryAdd(CurrencyType type, int amount, CurrencyAmplitudeAnalytic.SourceType sourceType = CurrencyAmplitudeAnalytic.SourceType.None, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	int GetCount(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	bool HasContainer(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	bool TryGetCountReactiveProperty(CurrencyType currency, out IReadOnlyReactiveProperty<int> count, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	IReadOnlyReactiveProperty<int> GetCountReactiveProperty(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	IDisposable GetSpendStream(CurrencyType type, Action<int> listener, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	IObservable<int> GetSpendObservable(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	IDisposable GetAddStream(CurrencyType type, Action<int> listener, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	bool TryReset(CurrencyType type, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));

	IDisposable GetChangeStream(CurrencyType type, Action<int> listener, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator));
}
