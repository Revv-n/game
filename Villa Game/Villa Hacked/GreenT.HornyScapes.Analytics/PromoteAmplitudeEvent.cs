using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Analytics;

public sealed class PromoteAmplitudeEvent : AmplitudeEvent
{
	private const string GirlPromote = "girl_level";

	private const string GirldId = "id";

	private const string GirlLevel = "level";

	private const string GirlRarity = "rarity";

	private const string ContentType = "content_type";

	public PromoteAmplitudeEvent(ICard girldCard, int level)
		: base("girl_level")
	{
		((AnalyticsEvent)this).AddEventParams("id", (object)girldCard.ID);
		((AnalyticsEvent)this).AddEventParams("level", (object)level);
		((AnalyticsEvent)this).AddEventParams("rarity", (object)girldCard.Rarity.ToString());
		((AnalyticsEvent)this).AddEventParams("content_type", (object)girldCard.ContentType.ToString());
	}
}
