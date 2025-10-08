using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Monetization;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes;

[MementoHolder]
public class PlayerPaymentsStats : ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public bool IsFirstStartPassed;

		public bool IsCohortWrongValuesCleared { get; private set; }

		public List<decimal> PaymentsPrices { get; private set; } = new List<decimal>();


		public Memento(PlayerPaymentsStats stats)
			: base(stats)
		{
			IsCohortWrongValuesCleared = stats.IsCohortWrongValuesCleared;
			PaymentsPrices = stats.PaymentsPrices.ToList();
			IsFirstStartPassed = stats.IsFirstStartPassed;
		}
	}

	public readonly Subject<decimal> OnAddNewPaymentUpdate = new Subject<decimal>();

	public readonly Subject<decimal> OnCohortPaymentStatsUpdate = new Subject<decimal>();

	public bool IsFirstStartPassed;

	private decimal paymentsAverage;

	private IRegionPriceResolver priceResolver;

	public List<decimal> PaymentsPrices { get; private set; } = new List<decimal>();


	public bool IsCohortWrongValuesCleared { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public PlayerPaymentsStats(IRegionPriceResolver priceResolver)
	{
		this.priceResolver = priceResolver;
	}

	public void AddBought(Lot lot)
	{
		if (lot is ValuableLot<decimal> valuableLot && valuableLot.Price.Currency == CurrencyType.Real && !valuableLot.IsFree)
		{
			decimal num = ConvertCohortToPlatform(valuableLot);
			PaymentsPrices.Add(num);
			paymentsAverage = CalculateSumPriceAverage();
			OnAddNewPaymentUpdate.OnNext(num);
			LogSumPriceAverage();
			OnCohortPaymentStatsUpdate.OnNext(paymentsAverage);
		}
	}

	public decimal GetSumPriceAverage()
	{
		return paymentsAverage;
	}

	private decimal ConvertCohortToPlatform(ValuableLot<decimal> valuableLot)
	{
		if (PlatformHelper.IsErolabsMonetization())
		{
			string paymentID = ((IPaymentID)valuableLot).PaymentID;
			return priceResolver.GetPriceConvertedToUS(paymentID);
		}
		return valuableLot.Price.Value;
	}

	public void Init()
	{
		InitAnalyticCohortsForOldUsers();
	}

	public void Dispose()
	{
		OnAddNewPaymentUpdate?.Dispose();
	}

	private void LogSumPriceAverage()
	{
	}

	private decimal CalculateSumPriceAverage()
	{
		if (!PaymentsPrices.Any())
		{
			return 0m;
		}
		return PaymentsPrices.Average();
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		if (memento2.PaymentsPrices == null)
		{
			return;
		}
		PaymentsPrices = memento2.PaymentsPrices;
		IsCohortWrongValuesCleared = memento2.IsCohortWrongValuesCleared;
		if (!IsCohortWrongValuesCleared)
		{
			CalculateSumPriceAverage();
			PaymentsPrices.RemoveAll((decimal x) => x % 10m == 0m);
			CalculateSumPriceAverage();
			IsCohortWrongValuesCleared = true;
		}
		IsFirstStartPassed = memento2.IsFirstStartPassed;
		paymentsAverage = CalculateSumPriceAverage();
		LogSumPriceAverage();
	}

	private void InitAnalyticCohortsForOldUsers()
	{
		if (IsFirstStartPassed)
		{
			return;
		}
		if (PaymentsPrices.Any())
		{
			List<decimal> list = new List<decimal>();
			foreach (decimal paymentsPrice in PaymentsPrices)
			{
				list.Add(paymentsPrice);
				decimal num = list.Average();
				OnCohortPaymentStatsUpdate.OnNext(num);
			}
		}
		else
		{
			OnCohortPaymentStatsUpdate.OnNext(0m);
		}
		IsFirstStartPassed = true;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public string UniqueKey()
	{
		return "player_payment_stats";
	}
}
