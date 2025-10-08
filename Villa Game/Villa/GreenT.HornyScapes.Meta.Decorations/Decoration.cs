using System;
using GreenT.Data;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Meta.Decorations;

[MementoHolder]
public class Decoration : ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int ID;

		public int State;

		public Memento(Decoration decoration)
			: base(decoration)
		{
			ID = decoration.ID;
			State = (int)decoration.State;
		}
	}

	private const string uniqueKeyPrefix = "RewardRoomObject.";

	private Subject<Decoration> onUpdate = new Subject<Decoration>();

	private readonly string uniqueKey;

	public IObservable<Decoration> OnUpdate => onUpdate.AsObservable();

	public bool IsRewarded => State == EntityStatus.Rewarded;

	public int ID { get; }

	public CompositeIdentificator HouseObjectID { get; }

	public ILocker DisplayCondition { get; }

	public EntityStatus State { get; private set; } = EntityStatus.Blocked;


	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Decoration(int id, CompositeIdentificator houseObjectID, ILocker locker)
	{
		ID = id;
		HouseObjectID = houseObjectID;
		DisplayCondition = locker;
		uniqueKey = "RewardRoomObject." + ID;
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
		onUpdate?.OnCompleted();
		onUpdate?.Dispose();
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		State = (EntityStatus)memento2.State;
		onUpdate.OnNext(this);
	}
}
