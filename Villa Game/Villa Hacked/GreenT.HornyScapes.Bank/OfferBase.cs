using System;
using GreenT.Data;
using GreenT.HornyScapes.Bank.Data;
using GreenT.Model.Collections;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Bank;

[MementoHolder]
public abstract class OfferBase : IBankSection, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public DateTime SaveTime { get; }

		public Memento(OfferSettings offer)
			: base(offer)
		{
			SaveTime = offer.clock.GetTime();
		}
	}

	public class Manager<T> : SimpleManager<T> where T : OfferBase
	{
	}

	private readonly string uniqueKey;

	protected readonly IClock clock;

	public int ID { get; }

	public BundleLot[] Bundles { get; }

	public LayoutType Layout { get; }

	public ILocker Lock { get; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public OfferBase(OfferMapper mapper, BundleLot[] offerBundles, CompositeLocker locker, IClock clock)
		: this(mapper.id, offerBundles, mapper.layout_type, locker, clock)
	{
	}

	public OfferBase(int id, BundleLot[] bundles, LayoutType layoutType, ILocker locker, IClock clock)
	{
		this.clock = clock;
		ID = id;
		Bundles = bundles;
		Layout = layoutType;
		Lock = locker;
		uniqueKey = "offer." + ID;
	}

	public override string ToString()
	{
		return "Offer: " + ID;
	}

	public virtual string UniqueKey()
	{
		return uniqueKey;
	}

	public abstract GreenT.Data.Memento SaveState();

	public virtual void LoadState(GreenT.Data.Memento memento)
	{
		if (memento is Memento)
		{
			Memento memento2 = (Memento)memento;
			if ((clock.GetTime() - memento2.SaveTime).Ticks <= 0)
			{
				Debug.LogError("Offer saved time was later then current.");
			}
		}
	}
}
