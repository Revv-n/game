using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.AntiCheat;

public class CheaterAnalytic : BaseAnalytic, IInitializable
{
	private const string ANALYTIC_EVENT = "is_cheater";

	private readonly CheatEngineSearchService _cheatEngineSearchService;

	private CompositeDisposable _subscriptions;

	private GameStarter _gameStarter;

	private IDisposable _stream;

	public CheaterAnalytic(CheatEngineSearchService cheatEngineSearchService, GameStarter gameStarter, IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		_cheatEngineSearchService = cheatEngineSearchService;
		_subscriptions = new CompositeDisposable();
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.DistinctUntilChanged<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool isActive) => isActive))), (Action<bool>)delegate
		{
			StartCheatMonitoring();
		}), (ICollection<IDisposable>)_subscriptions);
	}

	private void StartCheatMonitoring()
	{
		_stream?.Dispose();
		_stream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_cheatEngineSearchService.CheatEngineDetected, (Func<bool, bool>)((bool detected) => detected)), (Action<bool>)delegate
		{
			SendEvent();
		}), (ICollection<IDisposable>)_subscriptions);
	}

	private void SendEvent()
	{
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(new AmplitudeEvent("is_cheater"));
	}

	public override void Dispose()
	{
		CompositeDisposable subscriptions = _subscriptions;
		if (subscriptions != null)
		{
			subscriptions.Dispose();
		}
		base.Dispose();
	}
}
