namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventRouletteAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "miniEvent_Roulette";

	private const string ROULETTE_ID = "roulette_id";

	private const string ROULETTE_SOURCE = "roulette_source";

	public MiniEventRouletteAmplitudeEvent(int rouletteId, string source)
		: base("miniEvent_Roulette")
	{
		AddEventParams("roulette_id", rouletteId);
		AddEventParams("roulette_source", source);
	}
}
