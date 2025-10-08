using System;
using GreenT.Data;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Lockers;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank;

[MementoHolder]
public class GoldenTicket : ISavableState, IPurchasable, ILateDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public bool WasShowing { get; }

		public DateTime ShowStartDate { get; }

		public DateTime RespawnStartDate { get; }

		public Memento(GoldenTicket goldenTicket)
			: base(goldenTicket)
		{
			WasShowing = goldenTicket.isTimerOn;
			ShowStartDate = goldenTicket.showStartDate;
			RespawnStartDate = goldenTicket.respawnStartDate;
		}
	}

	public readonly IClock clock;

	public LinkedContent Content;

	public bool isTimerOn;

	public bool wasShowing;

	private DateTime showStartDate;

	private DateTime respawnStartDate;

	private CompositeDisposable timeStream;

	private string uniqueKey;

	public int ID { get; }

	public int ShowPriority { get; }

	public BundleLot Lot { get; }

	public Price<decimal> Price { get; }

	public ILocker Lock { get; }

	public ILocker LockWithTimer { get; }

	public DropSettings EnergyDrop { get; }

	public TimeSpan ShowTime { get; }

	public TimeSpan ShowTimeLeft { get; set; }

	public TimeSpan RespawnTime { get; }

	public TimeSpan TimeUntilRespawn { get; private set; }

	public GenericTimer RespawnTimer { get; }

	public float RespawnTimeDelta { get; }

	public TimeLocker DisplayTimeLocker { get; }

	public bool IsAvailableToShow
	{
		get
		{
			if (Lock.IsOpen.Value)
			{
				return ShowTimeLeft > TimeSpan.Zero;
			}
			return false;
		}
	}

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public GoldenTicket(GoldenTicketMapper mapper, BundleLot lot, CompositeLocker locker, CompositeLocker lockerWithTimer, TimeLocker timeLocker, IClock clock)
		: this(mapper.id, mapper.priority, lot, TimeSpan.FromSeconds(mapper.time), TimeSpan.FromSeconds(mapper.repeat_time), mapper.repeat_delta, locker, lockerWithTimer, timeLocker, clock)
	{
	}

	public GoldenTicket(int id, int priority, BundleLot lot, TimeSpan showTime, TimeSpan respawnTime, float respawnTimeDelta, ILocker locker, ILocker lockWithTimer, TimeLocker timeLocker, IClock clock)
	{
		ID = id;
		ShowPriority = priority;
		Lot = lot;
		Price = lot.Price;
		Content = lot.Content;
		EnergyDrop = GetEnergyDrop(Content);
		ShowTime = (ShowTimeLeft = showTime);
		RespawnTime = respawnTime;
		RespawnTimeDelta = respawnTimeDelta;
		DisplayTimeLocker = timeLocker;
		RespawnTimer = new GenericTimer();
		timeStream = new CompositeDisposable();
		Lock = locker;
		LockWithTimer = lockWithTimer;
		uniqueKey = "gticket." + ID;
		this.clock = clock;
	}

	public void Initialize()
	{
		StopTimers();
		isTimerOn = false;
		ShowTimeLeft = ShowTime;
		TimeUntilRespawn = RespawnTime;
	}

	public void LaunchTimers()
	{
		timeStream.Clear();
		LaunchDisplayTimer();
		if (RespawnTime != TimeSpan.Zero)
		{
			LaunchRespawnTimer();
		}
	}

	public void StopTimers()
	{
		timeStream.Clear();
		DisplayTimeLocker.Timer.Stop();
		RespawnTimer.Stop();
	}

	public bool Purchase()
	{
		return Lot.Purchase();
	}

	public void SendPurchaseNotification()
	{
		Lot.SendPurchaseNotification();
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		if (memento is Memento memento2)
		{
			showStartDate = memento2.ShowStartDate;
			respawnStartDate = memento2.RespawnStartDate;
			wasShowing = memento2.WasShowing;
		}
		SetState();
	}

	public void SetState()
	{
		DateTime time = clock.GetTime();
		if (showStartDate == default(DateTime))
		{
			ShowTimeLeft = ShowTime;
			TimeUntilRespawn = TimeSpan.Zero;
			return;
		}
		if (RespawnTime != TimeSpan.Zero)
		{
			TimeSpan timeSpan = time - respawnStartDate;
			if (timeSpan < RespawnTime)
			{
				TimeUntilRespawn = timeSpan - RespawnTime;
				ShowTimeLeft = TimeSpan.Zero;
				LaunchRespawnTimer();
			}
			else
			{
				TimeUntilRespawn = TimeSpan.Zero;
				ShowTimeLeft = ShowTime;
			}
		}
		if (wasShowing)
		{
			TimeSpan timeSpan2 = time - showStartDate;
			ShowTimeLeft = ((timeSpan2 < ShowTime) ? (ShowTime - timeSpan2) : TimeSpan.Zero);
		}
		DisplayTimeLocker.Timer.InitTime = ShowTime;
		DisplayTimeLocker.Timer.TimeLeft = ShowTimeLeft;
	}

	private DropSettings GetEnergyDrop(LinkedContent content)
	{
		DropSettings result = null;
		if (content is LootboxLinkedContent lootboxLinkedContent)
		{
			result = lootboxLinkedContent.Lootbox.GuarantedDrop.Find((DropSettings drop) => drop.Selector is CurrencySelector currencySelector && currencySelector.Currency == CurrencyType.Energy);
		}
		return result;
	}

	private void LaunchDisplayTimer()
	{
		isTimerOn = true;
		if (ShowTimeLeft == ShowTime)
		{
			showStartDate = clock.GetTime();
		}
		DisplayTimeLocker.Start(ShowTimeLeft);
		DisplayTimeLocker.Timer.OnUpdate.DoOnCancel(SetTimerAsOff).DoOnTerminate(SetTimerAsOff).Subscribe(delegate(TimeSpan _timeLeft)
		{
			ShowTimeLeft = _timeLeft;
		}, CatchException, SetTimerAsOff)
			.AddTo(timeStream);
		DisplayTimeLocker.Timer.OnTimeIsUp.Subscribe(delegate
		{
			SetTimerAsOff();
		}).AddTo(timeStream);
		void CatchException(Exception ex)
		{
			SetTimerAsOff();
			ex.LogException();
		}
		void SetTimerAsOff()
		{
			isTimerOn = false;
		}
	}

	private void LaunchRespawnTimer()
	{
		GenericTimer timer = DisplayTimeLocker.Timer;
		IObservable<GenericTimer> source = timer.OnTimeIsUp;
		if (ShowTimeLeft == TimeSpan.Zero)
		{
			source = source.StartWith(timer);
		}
		source.Subscribe(delegate
		{
			InitRespawnTimer();
		}).AddTo(timeStream);
		RespawnTimer.OnTimeIsUp.Subscribe(delegate
		{
			ShowTimeLeft = ShowTime;
		});
	}

	private void InitRespawnTimer()
	{
		if (TimeUntilRespawn <= TimeSpan.Zero)
		{
			TimeUntilRespawn = GetRandomTimeUntilRespawn();
			respawnStartDate = clock.GetTime();
		}
		RespawnTimer.Start(TimeUntilRespawn);
	}

	private TimeSpan GetRandomTimeUntilRespawn()
	{
		float num = UnityEngine.Random.Range(0f, RespawnTimeDelta);
		return RespawnTime + TimeSpan.FromSeconds(num);
	}

	public override string ToString()
	{
		return "GoldenTicket: " + ID + " Is visible:" + LockWithTimer.IsOpen.Value;
	}

	public void LateDispose()
	{
		timeStream?.Dispose();
		DisplayTimeLocker.Dispose();
		RespawnTimer.Dispose();
	}
}
