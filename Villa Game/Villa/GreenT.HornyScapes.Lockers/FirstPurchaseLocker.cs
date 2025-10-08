using System;
using GreenT.Data;

namespace GreenT.HornyScapes.Lockers;

[MementoHolder]
public class FirstPurchaseLocker : LotIsBoughtLocker, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public bool Value { get; private set; }

		public Memento(FirstPurchaseLocker firstPurchaseLocker)
			: base(firstPurchaseLocker)
		{
			Value = firstPurchaseLocker.isOpen.Value;
		}
	}

	private readonly string uniqueKey;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public FirstPurchaseLocker(bool openOnEvent)
		: base(openOnEvent)
	{
		uniqueKey = "first_purchase_locker_" + openOnEvent;
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		isOpen.Value = memento2.Value;
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
