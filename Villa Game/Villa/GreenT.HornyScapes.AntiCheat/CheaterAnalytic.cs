using System;
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
		_cheatEngineSearchService = cheatEngineSearchService;
		_subscriptions = new CompositeDisposable();
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		_gameStarter.IsGameActive.Where((bool isActive) => isActive).DistinctUntilChanged().Subscribe(delegate
		{
			StartCheatMonitoring();
		})
			.AddTo(_subscriptions);
	}

	private void StartCheatMonitoring()
	{
		_stream?.Dispose();
		_stream = _cheatEngineSearchService.CheatEngineDetected.Where((bool detected) => detected).Subscribe(delegate
		{
			SendEvent();
		}).AddTo(_subscriptions);
	}

	private void SendEvent()
	{
		amplitude.AddEvent(new AmplitudeEvent("is_cheater"));
	}

	public override void Dispose()
	{
		_subscriptions?.Dispose();
		base.Dispose();
	}
}
