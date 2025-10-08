using System;
using GreenT.Data;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

[MementoHolder]
public sealed class UserStats : ISavableState, IInitializable, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public DateTime StartTime { get; private set; }

		[field: SerializeField]
		public DateTime LastPayment { get; private set; }

		[field: SerializeField]
		public int PaymentsCount { get; private set; }

		public Memento(UserStats stats)
			: base(stats)
		{
			StartTime = stats.StartTime;
			LastPayment = stats.LastPayment;
			PaymentsCount = stats.PaymentsCount;
		}
	}

	private const string uniqueKey = "user.stats";

	public Subject<UserStats> onUpdate = new Subject<UserStats>();

	private IDisposable disposable;

	private readonly User user;

	private readonly IClock clock;

	private readonly GameStarter gameStarter;

	public bool IsDeveloper => false;

	public DateTime StartTime { get; private set; }

	public DateTime LastPayment { get; private set; }

	public TimeSpan InGameTime => clock.GetTime() - StartTime;

	public TimeSpan TimeWithoutPayments => clock.GetTime() - LastPayment;

	public int PaymentsCount { get; private set; }

	public IObservable<UserStats> OnUpdate => onUpdate.AsObservable();

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public UserStats(User user, IClock clock, GameStarter gameStarter)
	{
		this.user = user;
		this.clock = clock;
		this.gameStarter = gameStarter;
	}

	private void SetStatsToDefault()
	{
		StartTime = clock.GetTime();
		LastPayment = default(DateTime);
		PaymentsCount = 0;
		onUpdate.OnNext(this);
	}

	public void Initialize()
	{
		disposable?.Dispose();
		IObservable<Unit> observable = gameStarter.IsDataLoaded.FirstOrDefault((bool x) => x).AsUnitObservable().Share();
		disposable = observable.Merge(observable.ContinueWith(user.OnLogin.AsUnitObservable())).Subscribe(delegate
		{
			SetStatsToDefault();
		});
	}

	public void UpdatePaymentStats(LotBoughtSignal lotBoughtSignal)
	{
		if (lotBoughtSignal.Lot is ValuableLot<decimal> valuableLot && valuableLot.Price.Currency == CurrencyType.Real)
		{
			LastPayment = clock.GetTime();
			int paymentsCount = PaymentsCount + 1;
			PaymentsCount = paymentsCount;
			onUpdate.OnNext(this);
		}
	}

	public string UniqueKey()
	{
		return "user.stats";
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		StartTime = memento2.StartTime;
		LastPayment = memento2.LastPayment;
		PaymentsCount = memento2.PaymentsCount;
		onUpdate.OnNext(this);
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void Dispose()
	{
		disposable?.Dispose();
		onUpdate.OnCompleted();
		onUpdate.Dispose();
	}
}
