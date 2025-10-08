using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Lockers;
using GreenT.Model.Collections;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank;

[MementoHolder]
public class OfferSettings : ISavableState, IBankSection, ILateDisposable
{
	[Serializable]
	public class OfferSettingsMemento : TimeLimitedOffer.Memento
	{
		public OfferSettingsMemento(OfferSettings offer)
			: base(offer)
		{
		}
	}

	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public TimeSpan TimeLeft { get; }

		public TimeSpan TimeUntilRespawn { get; } = new TimeSpan(0L);


		public DateTime SaveTime { get; }

		public bool IsTimerOn { get; }

		public Memento(OfferSettings offer)
			: base(offer)
		{
			TimeLeft = offer.ShowTimeLeft;
			SaveTime = offer.clock.GetTime();
			IsTimerOn = offer.isTimerOn;
			TimeUntilRespawn = offer.TimeUntilRespawn;
		}
	}

	public class Manager : SimpleManager<OfferSettings>
	{
		public void Initialize()
		{
			foreach (OfferSettings item in Collection)
			{
				item.Initialize();
			}
		}
	}

	public class ViewSettings
	{
		private readonly BundlesProviderBase _bundlesProvider;

		private readonly ContentSource _contentSource;

		public string PresetKey { get; }

		public string BackgroundKey { get; }

		public ViewSettings(BundlesProviderBase bundlesProvider, ContentSource contentSource, string view_parameters)
		{
			_bundlesProvider = bundlesProvider;
			_contentSource = contentSource;
			BackgroundKey = string.Empty;
			PresetKey = view_parameters;
			if (string.IsNullOrEmpty(view_parameters))
			{
				return;
			}
			string[] array = view_parameters.Split(';', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(':', StringSplitOptions.None);
				string text = array2[0];
				if (!(text == "preset"))
				{
					if (!(text == "bg"))
					{
						throw new ArgumentOutOfRangeException("Неверный view_parameter <b>параметр:\"" + array2[0] + "\"</b>.").LogException();
					}
					BackgroundKey = array2[1];
				}
				else
				{
					PresetKey = array2[1];
				}
			}
		}

		public ViewPreset GetPreset()
		{
			return _bundlesProvider.TryFindInConcreteBundle<ViewPreset>(_contentSource, PresetKey);
		}
	}

	public class ShowSettings
	{
		public TimeSpan Left { get; }

		public TimeSpan ShowTime { get; }

		public TimeSpan Respawn { get; }

		public float RespawnDeltaTime { get; }

		public ShowSettings(TimeSpan showTime, TimeSpan respawn, float respawnDeltaTime)
		{
			ShowTime = showTime;
			Respawn = respawn;
			RespawnDeltaTime = respawnDeltaTime;
		}

		public bool IsVisible()
		{
			return true;
		}
	}

	public readonly IClock clock;

	private CompositeDisposable timeStream;

	private TimeInstaller.TimerCollection _timerCollection;

	public bool isTimerOn;

	private Subject<OfferSettings> onPushed = new Subject<OfferSettings>();

	private Subject<OfferSettings> onClicked = new Subject<OfferSettings>();

	private readonly string uniqueKey;

	public int ID { get; }

	public BundleLot[] Bundles { get; }

	public int SortingNumber { get; }

	public TimeSpan ShowTime { get; }

	public TimeSpan ShowTimeLeft { get; set; }

	public TimeSpan Respawn { get; }

	public float RespawnDeltaTime { get; }

	public TimeSpan TimeUntilRespawn { get; private set; }

	public GenericTimer RespawnTimer { get; }

	public TimeSpan PushTime { get; }

	public TimeLocker DisplayTimeLocker { get; }

	public LayoutType Layout { get; }

	public ILocker Lock { get; }

	public ILocker LockWithTimer { get; }

	public bool ForcedLock { get; }

	public ViewSettings ViewSets { get; }

	public IObservable<OfferSettings> OnPushed => Observable.AsObservable<OfferSettings>((IObservable<OfferSettings>)onPushed);

	public IObservable<OfferSettings> OnClicked => Observable.AsObservable<OfferSettings>((IObservable<OfferSettings>)onClicked);

	public TimeSpan DiffTime { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public OfferSettings(OfferMapper mapper, BundleLot[] offerBundles, CompositeLocker locker, CompositeLocker lockerWithTimer, ViewSettings layoutParams, TimeLocker timeLocker, IClock clock, TimeInstaller.TimerCollection timerCollection)
		: this(mapper.id, mapper.position, offerBundles, mapper.layout_type, locker, lockerWithTimer, mapper.end_on_lock, layoutParams, TimeSpan.FromSeconds(mapper.time), TimeSpan.FromSeconds(mapper.repeat_time), mapper.repeat_delta, TimeSpan.FromSeconds(mapper.push_time), timeLocker, clock, timerCollection)
	{
	}

	public OfferSettings(int id, int sortingNumber, BundleLot[] bundles, LayoutType layoutType, ILocker locker, ILocker lockWithTimer, bool forcedLock, ViewSettings layoutParams, TimeSpan showTime, TimeSpan respawn, float respawnDeltaTime, TimeSpan pushTime, TimeLocker timeLocker, IClock clock, TimeInstaller.TimerCollection timerCollection)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		ID = id;
		SortingNumber = sortingNumber;
		Layout = layoutType;
		Lock = locker;
		LockWithTimer = lockWithTimer;
		ForcedLock = forcedLock;
		Bundles = bundles;
		ViewSets = layoutParams;
		ShowTime = (ShowTimeLeft = showTime);
		Respawn = respawn;
		RespawnDeltaTime = respawnDeltaTime;
		PushTime = pushTime;
		DisplayTimeLocker = timeLocker;
		this.clock = clock;
		RespawnTimer = new GenericTimer();
		timeStream = new CompositeDisposable();
		_timerCollection = timerCollection;
		_timerCollection.Add(DisplayTimeLocker.Timer);
		_timerCollection.Add(RespawnTimer);
		uniqueKey = "offer." + ID;
	}

	private void LaunchDisplayTimer()
	{
		isTimerOn = true;
		DisplayTimeLocker.Start(ShowTimeLeft);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TimeSpan>(Observable.DoOnTerminate<TimeSpan>(Observable.DoOnCancel<TimeSpan>(DisplayTimeLocker.Timer.OnUpdate, (Action)SetTimerAsOff), (Action)SetTimerAsOff), (Action<TimeSpan>)delegate(TimeSpan _timeLeft)
		{
			ShowTimeLeft = _timeLeft;
		}, (Action<Exception>)CatchException, (Action)SetTimerAsOff), (ICollection<IDisposable>)timeStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GenericTimer>(DisplayTimeLocker.Timer.OnTimeIsUp, (Action<GenericTimer>)delegate
		{
			SetTimerAsOff();
		}), (ICollection<IDisposable>)timeStream);
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

	public void LaunchTimers()
	{
		timeStream.Clear();
		LaunchDisplayTimer();
		if (Respawn != TimeSpan.Zero)
		{
			LaunchRespawnTimer();
		}
	}

	private void LaunchRespawnTimer()
	{
		GenericTimer timer = DisplayTimeLocker.Timer;
		IObservable<GenericTimer> observable = timer.OnTimeIsUp;
		if (ShowTimeLeft == TimeSpan.Zero)
		{
			observable = Observable.StartWith<GenericTimer>(observable, timer);
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GenericTimer>(observable, (Action<GenericTimer>)delegate
		{
			InitRespawnTimer();
		}), (ICollection<IDisposable>)timeStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GenericTimer>(RespawnTimer.OnTimeIsUp, (Action<GenericTimer>)delegate
		{
			DisplayTimeLocker.Start(ShowTime);
		}), (ICollection<IDisposable>)timeStream);
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

	public bool IsAvailableToRespawn()
	{
		if (Respawn > TimeSpan.Zero)
		{
			return ShowTimeLeft <= TimeSpan.Zero;
		}
		return false;
	}

	public void StopTimers()
	{
		timeStream.Clear();
		DisplayTimeLocker.Timer.Stop();
		RespawnTimer.Stop();
	}

	public void Initialize()
	{
		StopTimers();
		isTimerOn = false;
		ShowTimeLeft = ShowTime;
		TimeUntilRespawn = Respawn;
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public virtual GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public virtual void LoadState(GreenT.Data.Memento memento)
	{
		if (memento is Memento memento2)
		{
			Memento memento3 = memento2;
			TimeSpan timeSpan = ((memento3.TimeLeft > TimeSpan.Zero) ? memento3.TimeLeft : TimeSpan.Zero);
			TimeUntilRespawn = memento3.TimeUntilRespawn;
			DateTime time = clock.GetTime();
			DiffTime = time - memento3.SaveTime;
			if (DiffTime.Ticks > 0)
			{
				if (memento3.IsTimerOn)
				{
					timeSpan -= DiffTime;
				}
				TimeUntilRespawn -= DiffTime;
			}
			if (memento3.TimeLeft < TimeSpan.Zero)
			{
				timeSpan = ShowTime;
				TimeUntilRespawn = TimeSpan.Zero;
			}
			else if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = TimeSpan.Zero;
			}
			ShowTimeLeft = ((timeSpan > ShowTime) ? ShowTime : timeSpan);
			DisplayTimeLocker.Timer.InitTime = ShowTime;
			DisplayTimeLocker.Timer.TimeLeft = ShowTimeLeft;
		}
		else
		{
			if (!(memento is OfferSettingsMemento offerSettingsMemento))
			{
				return;
			}
			OfferSettingsMemento offerSettingsMemento2 = offerSettingsMemento;
			TimeSpan timeSpan2 = ((offerSettingsMemento2.TimeLeft > TimeSpan.Zero) ? offerSettingsMemento2.TimeLeft : TimeSpan.Zero);
			TimeUntilRespawn = offerSettingsMemento2.TimeUntilRespawn;
			TimeSpan timeSpan3 = clock.GetTime() - offerSettingsMemento2.SaveTime;
			if (timeSpan3.Ticks > 0)
			{
				if (offerSettingsMemento2.IsTimerOn)
				{
					timeSpan2 -= timeSpan3;
				}
				TimeUntilRespawn -= timeSpan3;
			}
			if (offerSettingsMemento2.TimeLeft < TimeSpan.Zero)
			{
				timeSpan2 = ShowTime;
				TimeUntilRespawn = TimeSpan.Zero;
			}
			else if (timeSpan2 < TimeSpan.Zero)
			{
				timeSpan2 = TimeSpan.Zero;
			}
			ShowTimeLeft = ((timeSpan2 > ShowTime) ? ShowTime : timeSpan2);
			DisplayTimeLocker.Timer.InitTime = ShowTime;
			DisplayTimeLocker.Timer.TimeLeft = ShowTimeLeft;
		}
	}

	public void LateDispose()
	{
		CompositeDisposable obj = timeStream;
		if (obj != null)
		{
			obj.Dispose();
		}
		DisplayTimeLocker.Dispose();
		RespawnTimer.Dispose();
	}

	public override string ToString()
	{
		return "Offer: " + ID + " Position:" + SortingNumber + " Is visible:" + LockWithTimer.IsOpen.Value;
	}

	public void InvokeOnClickedEvent()
	{
		onClicked.OnNext(this);
	}

	public void InvokeOnPushedEvent()
	{
		onPushed.OnNext(this);
	}
}
