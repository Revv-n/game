using System;
using System.Collections.Generic;
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

	public IReadOnlyReactiveProperty<bool> CheatEngineDetected => (IReadOnlyReactiveProperty<bool>)(object)_cheatEngineDetected;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public CheatEngineSearchService(Saver saver, GameStarter gameStarter, UpdateCheatingStatusRequest updateCheatingStatusRequest)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		saver.Add(this);
		_updateCheatingStatusRequest = updateCheatingStatusRequest;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		_disposable.Clear();
		IObservable<bool> observable = Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>(Observable.SelectMany<bool, bool>(Observable.Where<bool>(observable, (Func<bool, bool>)((bool _) => !_cheatEngineDetected.Value)), (Func<bool, IObservable<bool>>)((bool _) => (IObservable<bool>)_cheatEngineDetected)), (Func<bool, bool>)((bool isDetected) => isDetected)), 1), (Action<bool>)delegate
		{
			_updateCheatingStatusRequest.GetRequest(isCheater: true);
		}), (ICollection<IDisposable>)_disposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(Observable.Do<bool>(Observable.DistinctUntilChanged<bool>(Observable.Select<long, bool>(Observable.TakeUntil<long, bool>(Observable.SelectMany<bool, long>(observable, (Func<bool, IObservable<long>>)((bool _) => Observable.Interval(TimeSpan.FromSeconds(_checkInterval)))), Observable.Where<bool>((IObservable<bool>)_cheatEngineDetected, (Func<bool, bool>)((bool x) => x))), (Func<long, bool>)((long _) => IsCheatEngineOpen()))), (Action<bool>)delegate(bool isDetected)
		{
			_cheatEngineDetected.Value = isDetected;
		}), (Func<bool, bool>)((bool value) => value)), (Action<bool>)delegate
		{
			OnCheatEngineDetected();
		}), (ICollection<IDisposable>)_disposable);
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
