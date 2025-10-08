using System;
using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes.Lootboxes;

public interface ILootboxOpener
{
	IObservable<Lootbox> OnOpen { get; }

	void Open(Lootbox lootbox, CurrencyAmplitudeAnalytic.SourceType sourceType);

	void Open(int lootboxID, CurrencyAmplitudeAnalytic.SourceType sourceType);
}
