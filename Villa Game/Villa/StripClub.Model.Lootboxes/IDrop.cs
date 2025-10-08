using GreenT.HornyScapes.Analytics;

namespace StripClub.Model.Lootboxes;

public interface IDrop
{
	LinkedContent GetDrop(CurrencyAmplitudeAnalytic.SourceType SourceType);
}
