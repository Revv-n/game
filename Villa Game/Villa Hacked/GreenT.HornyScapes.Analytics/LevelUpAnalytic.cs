using System;
using System.Collections.Generic;
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
		CompositeDisposable obj = onNewStream;
		if (obj != null)
		{
			obj.Clear();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Skip<int>(Observable.SelectMany<bool, int>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool _) => _)), (Func<bool, IObservable<int>>)((bool _) => (IObservable<int>)battlePassLevelProvider.Level)), 1), (Action<int>)base.SendEventIfIsValid), (ICollection<IDisposable>)onNewStream);
	}

	public override void SendEventByPass(int tuple)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("level");
		((AnalyticsEvent)amplitudeEvent).AddEventParams("level", (object)tuple);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
	}
}
