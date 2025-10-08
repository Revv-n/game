using GreenT.HornyScapes.StarShop;

namespace GreenT.HornyScapes.Analytics.Starshops;

public class StarShopAmplitudeEventOld : AmplitudeEvent
{
	private const string EVENT_NAME_KEY = "step_completed";

	private const string ID_KEY = "step_completed";

	public StarShopAmplitudeEventOld(StarShopItem entity)
		: base("step_completed")
	{
		((AnalyticsEvent)this).AddEventParams("step_completed", (object)$"{entity.ID}");
	}
}
