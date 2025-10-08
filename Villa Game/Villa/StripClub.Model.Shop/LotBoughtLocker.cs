using System;
using GreenT.Data;

namespace StripClub.Model.Shop;

[MementoHolder]
public class LotBoughtLocker : Locker, ISavableState
{
	public enum Condition
	{
		Bought,
		NotBought
	}

	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public bool open;

		public Memento(LotBoughtLocker locker)
			: base(locker)
		{
			open = locker.IsOpen.Value;
		}
	}

	public readonly int targetID;

	public readonly Condition condition;

	private string uniqueKey;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public LotBoughtLocker(int lotID, Condition condition)
	{
		targetID = lotID;
		this.condition = condition;
		Initialize();
		uniqueKey = "lot_bought_locker" + targetID + "_" + condition;
	}

	public override void Initialize()
	{
		isOpen.Value = condition == Condition.NotBought;
	}

	public void Set(Lot lot)
	{
		if (lot.ID == targetID)
		{
			isOpen.Value = (condition == Condition.NotBought && lot.Received == 0) || (condition == Condition.Bought && lot.Received > 0);
		}
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		isOpen.Value = memento2.open;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}
}
