using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes.Presents.Analytics;

public class PresentSpendAnalyticEvent : AmplitudeEvent
{
	private const string EventTypeKey = "currency_present_spent";

	private const string AmountKey = "amount";

	private const string TypeKey = "type";

	private const string CharacterKey = "character";

	public PresentSpendAnalyticEvent(string presentId, int spendAmount, int characterId)
		: base("currency_present_spent")
	{
		((AnalyticsEvent)this).AddEventParams("amount", (object)spendAmount);
		((AnalyticsEvent)this).AddEventParams("type", (object)presentId);
		((AnalyticsEvent)this).AddEventParams("character", (object)characterId);
	}
}
