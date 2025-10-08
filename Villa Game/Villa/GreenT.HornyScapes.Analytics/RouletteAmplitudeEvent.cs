namespace GreenT.HornyScapes.Analytics;

public sealed class RouletteAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "roulette";

	private const string ROULETTE_ID = "roulette_id";

	private const string ROULETTE_SOURCE = "roulette_source";

	public RouletteAmplitudeEvent(int rouletteId, string source)
		: base("roulette")
	{
		AddEventParams("roulette_id", rouletteId);
		AddEventParams("roulette_source", source);
	}
}
