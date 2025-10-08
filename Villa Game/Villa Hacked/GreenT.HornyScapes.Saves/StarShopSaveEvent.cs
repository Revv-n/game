using System;
using System.Collections.Generic;
using GreenT.HornyScapes.StarShop;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class StarShopSaveEvent : SaveEvent
{
	public EntityStatus SaveOnState = EntityStatus.Rewarded;

	private StarShopManager manager;

	private GameStarter gameStarter;

	[Inject]
	public void Init(StarShopManager manager, GameStarter gameStarter)
	{
		this.manager = manager;
		this.gameStarter = gameStarter;
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StarShopItem>(Observable.Where<StarShopItem>(Observable.ContinueWith<bool, StarShopItem>(Observable.FirstOrDefault<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), manager.OnUpdate), (Func<StarShopItem, bool>)((StarShopItem _item) => _item.State == SaveOnState)), (Action<StarShopItem>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}

	public override void Track()
	{
		Initialize();
	}
}
