using System;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.Sounds;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.Meta;

public class RoomObjectsBuilder : IHouseBuilder, IDisposable
{
	private Subject<IRoomObject<BaseObjectConfig>> onCreated = new Subject<IRoomObject<BaseObjectConfig>>();

	private IAudioPlayer audioPlayer;

	private RoomManager house;

	private GameStarter gameStarter;

	public IObservable<IRoomObject<BaseObjectConfig>> OnCreated => onCreated.AsObservable();

	public RoomObjectsBuilder(IAudioPlayer audioPlayer, RoomManager house, GameStarter gameStarter)
	{
		this.audioPlayer = audioPlayer;
		this.house = house;
		this.gameStarter = gameStarter;
	}

	public void Dispose()
	{
		onCreated?.OnCompleted();
		onCreated?.Dispose();
	}

	public void BuildRoomObject(CompositeIdentificator id)
	{
		if (house.IsObjectSet(id))
		{
			IRoomObject<BaseObjectConfig> @object = house.GetObject(id);
			@object.SetStatus(EntityStatus.Rewarded);
			onCreated.OnNext(@object);
			if ((bool)@object.Config.ChangeStateSound && gameStarter.IsGameActive.Value)
			{
				audioPlayer?.PlayAudioClip2D(@object.Config.ChangeStateSound.Sound);
			}
		}
	}
}
