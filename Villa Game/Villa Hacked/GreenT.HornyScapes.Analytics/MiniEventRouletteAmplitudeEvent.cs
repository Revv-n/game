namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventRouletteAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "miniEvent_Roulette";

	private const string ROULETTE_ID = "roulette_id";

	private const string ROULETTE_SOURCE = "roulette_source";

	public MiniEventRouletteAmplitudeEvent(int rouletteId, string source)
		: base("miniEvent_Roulette")
	{
		((AnalyticsEvent)this).AddEventParams("roulette_id", (object)rouletteId);
		((AnalyticsEvent)this).AddEventParams("roulette_source", (object)source);
	}
}
