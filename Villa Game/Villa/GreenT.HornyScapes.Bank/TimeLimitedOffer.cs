using System;
using GreenT.Data;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Lockers;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank;

[MementoHolder]
public abstract class TimeLimitedOffer : OfferBase, ISavableState, ILateDisposable
{
	[Serializable]
	public new class Memento : OfferBase.Memento
	{
		public TimeSpan TimeLeft { get; }

		public TimeSpan TimeUntilRespawn { get; } = new TimeSpan(0L);


		public bool IsTimerOn { get; }

		public Memento(OfferSettings offer)
			: base(offer)
		{
			TimeLeft = offer.ShowTimeLeft;
			IsTimerOn = offer.isTimerOn;
			TimeUntilRespawn = offer.TimeUntilRespawn;
		}
	}

	public new class Manager<T> : OfferBase.Manager<T> where T : TimeLimitedOffer
	{
		public virtual void Initialize()
		{
			foreach (T item in Collection)
			{
				item.Initialize();
			}
		}
	}

	private CompositeDisposable timeStream;

	protected bool isTimerOn;

	public TimeSpan ShowTime { get; }

	public TimeSpan ShowTimeLeft { get; set; }

	public TimeSpan Respawn { get; }

	public float RespawnDeltaTime { get; }

	public TimeSpan TimeUntilRespawn { get; protected set; }

	public GenericTimer RespawnTimer { get; }

	public TimeLocker DisplayTimeLocker { get; }

	public ILocker LockWithTimer { get; }

	public TimeLimitedOffer(OfferMapper mapper, BundleLot[] offerBundles, CompositeLocker locker, CompositeLocker lockerWithTimer, TimeLocker timeLocker, IClock clock)
		: base(mapper, offerBundles, locker, clock)
	{
		LockWithTimer = lockerWithTimer;
		ShowTime = (ShowTimeLeft = TimeSpan.FromSeconds(mapper.time));
		DisplayTimeLocker = timeLocker;
		Respawn = TimeSpan.FromSeconds(mapper.repeat_time);
		RespawnDeltaTime = mapper.repeat_delta;
		RespawnTimer = new GenericTimer();
		timeStream = new CompositeDisposable();
	}

	public bool IsAvailableToRespawn()
	{
		if (Respawn > TimeSpan.Zero)
		{
			return ShowTimeLeft <= TimeSpan.Zero;
		}
		return false;
	}

	public virtual void Initialize()
	{
		StopTimers();
		isTimerOn = false;
		ShowTimeLeft = ShowTime;
		TimeUntilRespawn = Respawn;
	}

	private void InitRespawnTimer()
	{
		TimeUntilRespawn = GetRandomTimeUntilRespawn();
		RespawnTimer.Start(TimeUntilRespawn);
	}

	private TimeSpan GetRandomTimeUntilRespawn()
	{
		float num = UnityEngine.Random.Range(0f, RespawnDeltaTime);
		return Respawn + TimeSpan.FromSeconds(num);
	}

	public void LaunchTimers()
	{
		timeStream.Clear();
		LaunchDisplayTimer();
		if (Respawn != TimeSpan.Zero)
		{
			LaunchRespawnTimer();
		}
	}

	private void LaunchDisplayTimer()
	{
		isTimerOn = true;
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
			DisplayTimeLocker.Start(ShowTime);
		}).AddTo(timeStream);
	}

	public void StopTimers()
	{
		timeStream.Clear();
		DisplayTimeLocker.Timer.Stop();
		RespawnTimer.Stop();
	}

	public virtual void LateDispose()
	{
		timeStream?.Dispose();
		DisplayTimeLocker.Dispose();
	}

	public override void LoadState(GreenT.Data.Memento memento)
	{
		base.LoadState(memento);
		Memento memento2 = (Memento)memento;
		ShowTimeLeft = memento2.TimeLeft;
		TimeUntilRespawn = memento2.TimeUntilRespawn;
		TimeSpan timeSpan = clock.GetTime() - memento2.SaveTime;
		if (timeSpan.Ticks > 0)
		{
			if (memento2.IsTimerOn)
			{
				ShowTimeLeft -= timeSpan;
			}
			TimeUntilRespawn -= timeSpan;
		}
	}
}
