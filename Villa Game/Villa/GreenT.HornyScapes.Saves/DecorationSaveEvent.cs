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
		(from item in _gameStarter.IsGameActive.FirstOrDefault((bool x) => x).ContinueWith(_decorationManager.OnUpdate)
			where item.State == _saveOnState
			select item).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}

	public override void Track()
	{
		Initialize();
	}

	private void OnDestroy()
	{
		saveStreams?.Dispose();
	}
}
