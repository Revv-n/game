using System;
using GreenT.Data;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Lockers;

[MementoHolder]
public class LotBoughtBySectionLocker : EqualityLocker<int>, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int lotBoughtCount;

		public Memento(LotBoughtBySectionLocker locker)
			: base(locker)
		{
			lotBoughtCount = locker.lotBoughtCount;
		}
	}

	public readonly int sectionID;

	public readonly Restriction restrictor;

	public int lotBoughtCount;

	private readonly string uniqueKey;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public LotBoughtBySectionLocker(int sectionID, int count, Restriction restrictor)
		: base(0, count, restrictor)
	{
		this.sectionID = sectionID;
		this.restrictor = restrictor;
		uniqueKey = "lot_bought_in_section" + sectionID + "_" + base.Value + "_" + restrictor;
	}

	public void Add(Lot lot)
	{
		if (lot.TabID == sectionID)
		{
			Set(++lotBoughtCount);
		}
	}

	public override void Initialize()
	{
		if (lotBoughtCount == 0)
		{
			base.Initialize();
		}
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		if (memento is Memento memento2)
		{
			lotBoughtCount = memento2.lotBoughtCount;
			Set(lotBoughtCount);
			if (base.Value == 99)
			{
				Debug.Log("load lotBoughtCount " + lotBoughtCount);
			}
		}
	}

	public GreenT.Data.Memento SaveState()
	{
		if (base.Value == 99)
		{
			Debug.Log("save lotBoughtCount " + lotBoughtCount);
		}
		return new Memento(this);
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}
}
