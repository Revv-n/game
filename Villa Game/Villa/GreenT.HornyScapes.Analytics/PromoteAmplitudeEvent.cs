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
		AddEventParams("id", girldCard.ID);
		AddEventParams("level", level);
		AddEventParams("rarity", girldCard.Rarity.ToString());
		AddEventParams("content_type", girldCard.ContentType.ToString());
	}
}
