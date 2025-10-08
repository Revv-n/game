namespace GreenT.HornyScapes.Analytics;

public sealed class RouletteAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "roulette";

	private const string ROULETTE_ID = "roulette_id";

	private const string ROULETTE_SOURCE = "roulette_source";

	public RouletteAmplitudeEvent(int rouletteId, string source)
		: base("roulette")
	{
		((AnalyticsEvent)this).AddEventParams("roulette_id", (object)rouletteId);
		((AnalyticsEvent)this).AddEventParams("roulette_source", (object)source);
	}
}
