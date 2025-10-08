using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class LevelUpAnalytic : BaseAnalytic<int>
{
	private const string ANALYTIC_EVENT = "level";

	private readonly BattlePassLevelProvider battlePassLevelProvider;

	private readonly GameStarter gameStarter;

	public LevelUpAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, BattlePassLevelProvider battlePassLevelProvider, GameStarter gameStarter)
		: base(amplitude)
	{
		this.battlePassLevelProvider = battlePassLevelProvider;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		onNewStream?.Clear();
		gameStarter.IsGameActive.Where((bool _) => _).SelectMany((bool _) => battlePassLevelProvider.Level).Skip(1)
			.Subscribe(base.SendEventIfIsValid)
			.AddTo(onNewStream);
	}

	public override void SendEventByPass(int tuple)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("level");
		amplitudeEvent.AddEventParams("level", tuple);
		amplitude.AddEvent(amplitudeEvent);
	}
}
