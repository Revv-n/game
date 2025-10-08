using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Constants;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Epocha;

[MementoHolder]
public class CrossPromoController : IInitializable, IDisposable, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public DateTime CrosspromoShowTime { get; private set; }

		public Memento(CrossPromoController controller)
			: base(controller)
		{
			CrosspromoShowTime = controller.ShowTime;
		}
	}

	private readonly User user;

	private readonly UserStats stats;

	private readonly IClock clock;

	private readonly GameStarter gameStarter;

	private readonly IConstants<int> constants;

	private TimeSpan crossPromoCooldownTime;

	private GenericTimer crossPromoTimer;

	private CompositeDisposable disposables = new CompositeDisposable();

	public DateTime ShowTime { get; private set; }

	public TimeSpan TimeUntilCrossPromo => crossPromoCooldownTime - (clock.GetTime() - ShowTime);

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public CrossPromoController(User user, UserStats stats, IClock clock, IConstants<int> constants, GameStarter gameStarter)
	{
		this.user = user;
		this.stats = stats;
		this.clock = clock;
		this.gameStarter = gameStarter;
		this.constants = constants;
	}

	public void Initialize()
	{
		disposables.Clear();
		IObservable<Unit> observable = CreateTimerStream();
		IConnectableObservable<Unit> connectableObservable = gameStarter.IsGameReadyToStart.Where((bool x) => x).AsUnitObservable().Publish();
		IObservable<Unit> observable2 = connectableObservable.SelectMany((Unit _) => stats.OnUpdate.TakeUntil(user.OnLogin)).AsUnitObservable().Debug(GetType().Name + ": обновились статы пользователя", LogType.Develop);
		connectableObservable.Debug(GetType().Name + ": игра готова к запуску", LogType.Develop).Merge(observable2).Merge(observable)
			.Subscribe(delegate
			{
				TryPushCrossPromo();
			})
			.AddTo(disposables);
		gameStarter.IsGameReadyToStart.Where((bool x) => !x && crossPromoTimer != null).Debug(GetType().Name + ": остановлен таймер", LogType.Develop).Subscribe(delegate
		{
			crossPromoTimer.Stop();
		})
			.AddTo(disposables);
		connectableObservable.Connect().AddTo(disposables);
	}

	private IObservable<Unit> CreateTimerStream()
	{
		IObservable<Unit> source = ((crossPromoTimer != null) ? crossPromoTimer.OnTimeIsUp.AsUnitObservable() : (from _ in gameStarter.IsDataLoaded.FirstOrDefault((bool x) => x)
			select CreateCrossPromoTimer()).SelectMany((GenericTimer _timer) => _timer.OnTimeIsUp.AsUnitObservable()));
		return source.Debug(GetType().Name + ": игра готова к запуску по таймеру", LogType.Develop);
	}

	private GenericTimer CreateCrossPromoTimer()
	{
		float num = constants["crosspromo_cooldown_seconds"];
		crossPromoCooldownTime = TimeSpan.FromSeconds(num);
		crossPromoTimer = new GenericTimer(crossPromoCooldownTime);
		return crossPromoTimer;
	}

	private void TryPushCrossPromo()
	{
	}

	public byte[] EvaluateCategories()
	{
		List<byte> list = new List<byte>();
		if (stats.IsDeveloper)
		{
			list.Add(6);
		}
		TimeSpan inGameTime = stats.InGameTime;
		TimeSpan timeWithoutPayments = stats.TimeWithoutPayments;
		int paymentsCount = stats.PaymentsCount;
		if (paymentsCount == 0)
		{
			if (inGameTime.Days > 21)
			{
				list.Add(1);
			}
			if (inGameTime.Days >= 4 && inGameTime.Days <= 21)
			{
				list.Add(4);
			}
			if (inGameTime.TotalMinutes >= 50.0 && inGameTime.Days < 3)
			{
				list.Add(7);
			}
		}
		else if (timeWithoutPayments.TotalDays > 4.0)
		{
			list.Add(5);
			if (timeWithoutPayments.TotalDays > 30.0)
			{
				if (paymentsCount == 2)
				{
					list.Add(2);
				}
				if (paymentsCount == 1)
				{
					list.Add(3);
				}
			}
		}
		return list.ToArray();
	}

	public void Dispose()
	{
		disposables.Dispose();
	}

	public string UniqueKey()
	{
		return "crosspromo.controller";
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		ShowTime = memento2.CrosspromoShowTime;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}
}
