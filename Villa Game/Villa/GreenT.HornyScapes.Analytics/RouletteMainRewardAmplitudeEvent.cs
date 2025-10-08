namespace GreenT.HornyScapes.Analytics;

public sealed class RouletteMainRewardAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "roulette_main_reward";

	private const string ROULETTE_ID = "roulette_id";

	private const string ROULETTE_SOURCE = "roulette_source";

	private const string ATTEMPT = "attempt";

	public RouletteMainRewardAmplitudeEvent(int rouletteId, int attempts, string source)
		: base("roulette_main_reward")
	{
		AddEventParams("roulette_id", rouletteId);
		AddEventParams("roulette_source", source);
		AddEventParams("attempt", attempts);
	}
}
