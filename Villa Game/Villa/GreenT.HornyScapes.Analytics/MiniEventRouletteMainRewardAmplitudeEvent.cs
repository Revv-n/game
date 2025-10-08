namespace GreenT.HornyScapes.Analytics;

public sealed class MiniEventRouletteMainRewardAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "miniEvent_Roulette_main_reward";

	private const string ROULETTE_ID = "roulette_id";

	private const string ROULETTE_SOURCE = "roulette_source";

	private const string ATTEMPT = "attempt";

	public MiniEventRouletteMainRewardAmplitudeEvent(int rouletteId, int attempts, string source)
		: base("miniEvent_Roulette_main_reward")
	{
		AddEventParams("roulette_id", rouletteId);
		AddEventParams("roulette_source", source);
		AddEventParams("attempt", attempts);
	}
}
