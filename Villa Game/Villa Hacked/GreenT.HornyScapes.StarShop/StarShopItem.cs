using System;
using GreenT.Data;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.StarShop;

[MementoHolder]
public class StarShopItem : IStarShopItem, ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int ID;

		public int State;

		public Memento(StarShopItem starShopItem)
			: base(starShopItem)
		{
			ID = starShopItem.ID;
			State = (int)starShopItem.State;
		}
	}

	protected Subject<StarShopItem> onUpdate = new Subject<StarShopItem>();

	private readonly string uniqueKey;

	public IObservable<StarShopItem> OnUpdate => Observable.AsObservable<StarShopItem>((IObservable<StarShopItem>)onUpdate);

	public int ID { get; }

	public int LootboxIdReward { get; }

	public CompositeIdentificator HouseObjectID { get; }

	public Cost Cost { get; }

	public CompositeLocker Lock { get; }

	public EntityStatus State { get; protected set; } = EntityStatus.Blocked;


	public SavableStatePriority Priority => SavableStatePriority.Base;

	public StarShopItem(int id, int reward, CompositeIdentificator houseObjectID, Cost cost, ILocker[] lockers)
	{
		ID = id;
		LootboxIdReward = reward;
		Cost = cost;
		Lock = new CompositeLocker(lockers);
		HouseObjectID = houseObjectID;
		uniqueKey = "StarShop." + ID;
	}

	public void Initialize()
	{
		State = EntityStatus.Blocked;
		onUpdate.OnNext(this);
	}

	public void SetState(EntityStatus state)
	{
		if (State != state && State != EntityStatus.Rewarded)
		{
			State = state;
			onUpdate.OnNext(this);
		}
	}

	public void Dispose()
	{
		onUpdate.OnCompleted();
		onUpdate.Dispose();
	}

	public override string ToString()
	{
		return GetType().Name + " ID: " + ID + " State: " + State;
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public virtual GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		State = (EntityStatus)memento2.State;
		if (State != EntityStatus.Rewarded && State != EntityStatus.Blocked && !Lock.IsOpen.Value)
		{
			State = EntityStatus.Blocked;
		}
		onUpdate.OnNext(this);
	}
}
