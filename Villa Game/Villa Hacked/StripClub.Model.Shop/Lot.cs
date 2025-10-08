using System;
using GreenT.Data;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.Model.Shop;

[MementoHolder]
public abstract class Lot : ISavableState, IPurchasable, ILotMonetizationData
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public int Received { get; private set; }

		[field: SerializeField]
		public bool IsViewed { get; private set; }

		public Memento(Lot lot)
			: base(lot)
		{
			Save(lot);
		}

		public GreenT.Data.Memento Save(Lot lot)
		{
			Received = lot.Received;
			IsViewed = lot.IsViewed;
			return this;
		}
	}

	protected readonly EqualityLocker<int> countLocker;

	private int received;

	private readonly Subject<Lot> onLotRecieved = new Subject<Lot>();

	public EqualityLocker<int> CountLocker => countLocker;

	protected static SignalBus SignalBus { get; private set; }

	public int ID { get; }

	public int MonetizationID { get; }

	public abstract string LocalizationKey { get; }

	public int TabID { get; }

	public int SerialNumber { get; protected set; }

	public int AvailableCount { get; }

	public abstract bool IsReal { get; }

	public int Received
	{
		get
		{
			return received;
		}
		protected set
		{
			bool num = received < value;
			received = value;
			countLocker.Set(received);
			if (num)
			{
				onLotRecieved.OnNext(this);
			}
		}
	}

	public ILocker Locker { get; }

	public bool IsViewed { get; set; }

	public abstract bool IsFree { get; }

	public abstract LinkedContent Content { get; }

	public string ShopSource { get; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public IObservable<Lot> OnLotReceived()
	{
		return Observable.AsObservable<Lot>((IObservable<Lot>)onLotRecieved);
	}

	public Lot(int id, int monetizationID, int tab_id, int position, int buy_times, ILocker locker, EqualityLocker<int> countLocker, string shopSource)
	{
		ID = id;
		MonetizationID = monetizationID;
		TabID = tab_id;
		SerialNumber = position;
		AvailableCount = buy_times;
		Locker = locker;
		this.countLocker = countLocker;
		ShopSource = shopSource;
	}

	public virtual void Initialize()
	{
		countLocker.Initialize();
		IsViewed = false;
		received = 0;
	}

	public static void Set(SignalBus signalBus)
	{
		SignalBus = signalBus;
	}

	public bool IsAvailable()
	{
		if (Received < AvailableCount || AvailableCount == 0)
		{
			return Locker.IsOpen.Value;
		}
		return false;
	}

	public void UpdateDailyInfo()
	{
		TryResetDailyInfo();
	}

	protected virtual void TryResetDailyInfo()
	{
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + ID + " Tab ID: " + TabID + " Number: " + SerialNumber + " Viewed:" + IsViewed + " Available:" + IsAvailable() + " Recieved: " + Received;
	}

	public abstract string UniqueKey();

	public virtual GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public virtual void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		Received = memento2.Received;
		IsViewed = memento2.IsViewed;
		UpdateDailyInfo();
	}

	public abstract bool Purchase();

	public virtual void SendPurchaseNotification()
	{
		SignalBus.Fire<LotBoughtSignal>(new LotBoughtSignal(this));
	}
}
