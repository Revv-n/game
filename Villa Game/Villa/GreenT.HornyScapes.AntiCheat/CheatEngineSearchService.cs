using System;
using GreenT.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.AntiCheat;

[MementoHolder]
public class CheatEngineSearchService : IInitializable, IDisposable, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public bool IsCheater { get; private set; }

		public Memento(CheatEngineSearchService service)
			: base(service)
		{
			IsCheater = service._cheatEngineDetected.Value;
		}
	}

	private readonly ReactiveProperty<bool> _cheatEngineDetected = new ReactiveProperty<bool>();

	private readonly UpdateCheatingStatusRequest _updateCheatingStatusRequest;

	private readonly GameStarter _gameStarter;

	private float _checkInterval = 30f;

	private CompositeDisposable _disposable = new CompositeDisposable();

	public IReadOnlyReactiveProperty<bool> CheatEngineDetected => _cheatEngineDetected;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public CheatEngineSearchService(Saver saver, GameStarter gameStarter, UpdateCheatingStatusRequest updateCheatingStatusRequest)
	{
		saver.Add(this);
		_updateCheatingStatusRequest = updateCheatingStatusRequest;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		_disposable.Clear();
		IObservable<bool> source = _gameStarter.IsGameActive.Where((bool x) => x);
		(from isDetected in source.Where((bool _) => !_cheatEngineDetected.Value).SelectMany((bool _) => _cheatEngineDetected)
			where isDetected
			select isDetected).Take(1).Subscribe(delegate
		{
			_updateCheatingStatusRequest.GetRequest(isCheater: true);
		}).AddTo(_disposable);
		(from value in (from _ in source.SelectMany((bool _) => Observable.Interval(TimeSpan.FromSeconds(_checkInterval))).TakeUntil(_cheatEngineDetected.Where((bool x) => x))
				select IsCheatEngineOpen()).DistinctUntilChanged().Do(delegate(bool isDetected)
			{
				_cheatEngineDetected.Value = isDetected;
			})
			where value
			select value).Subscribe(delegate
		{
			OnCheatEngineDetected();
		}).AddTo(_disposable);
	}

	private bool IsCheatEngineOpen()
	{
		return false;
	}

	private void OnCheatEngineDetected()
	{
		if (!_cheatEngineDetected.Value)
		{
			_cheatEngineDetected.Value = true;
			Debug.Log("Bad boy detected!");
		}
	}

	public void Dispose()
	{
		_disposable.Dispose();
	}

	public string UniqueKey()
	{
		return "anticheat.ischeater";
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		_cheatEngineDetected.Value = memento2.IsCheater;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}
}
