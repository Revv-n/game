using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Meta.Decorations;
using Merge.Meta.RoomObjects;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class DecorationSaveEvent : SaveEvent
{
	[SerializeField]
	private EntityStatus _saveOnState;

	private DecorationManager _decorationManager;

	private GameStarter _gameStarter;

	[Inject]
	public void Init(DecorationManager decorationManager, GameStarter gameStarter)
	{
		_decorationManager = decorationManager;
		_gameStarter = gameStarter;
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Decoration>(Observable.Where<Decoration>(Observable.ContinueWith<bool, Decoration>(Observable.FirstOrDefault<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), _decorationManager.OnUpdate), (Func<Decoration, bool>)((Decoration item) => item.State == _saveOnState)), (Action<Decoration>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}

	public override void Track()
	{
		Initialize();
	}

	private void OnDestroy()
	{
		CompositeDisposable obj = saveStreams;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
