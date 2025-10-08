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
		(from _item in gameStarter.IsGameActive.FirstOrDefault((bool x) => x).ContinueWith(manager.OnUpdate)
			where _item.State == SaveOnState
			select _item).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}

	public override void Track()
	{
		Initialize();
	}
}
