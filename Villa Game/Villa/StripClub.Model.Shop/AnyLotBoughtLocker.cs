using System;
using System.Linq;
using GreenT.Data;

namespace StripClub.Model.Shop;

[MementoHolder]
public class AnyLotBoughtLocker : Locker, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public bool open;

		public Memento(AnyLotBoughtLocker locker)
			: base(locker)
		{
			open = locker.IsOpen.Value;
		}
	}

	public readonly int[] targetID;

	public readonly LotBoughtLocker.Condition condition;

	private string uniqueKey;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public AnyLotBoughtLocker(int[] lotID, LotBoughtLocker.Condition condition)
	{
		targetID = lotID;
		this.condition = condition;
		Initialize();
		uniqueKey = "any_lot_bought_locker" + targetID?.ToString() + "_" + condition;
	}

	public override void Initialize()
	{
	}

	public void Set(Lot lot)
	{
		if (!isOpen.Value && targetID.Any((int target) => target == lot.ID))
		{
			isOpen.Value = lot.Received > 0;
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
