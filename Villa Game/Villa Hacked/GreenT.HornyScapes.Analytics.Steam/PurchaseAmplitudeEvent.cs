using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics.Steam;

public class PurchaseAmplitudeEvent : BasePurchaseAmplitudeEvent
{
	protected const string ORDER_KEY = "order_id";

	protected const string TRANSACTION_KEY = "transaction_id";

	public PurchaseAmplitudeEvent(SteamPaymentData data, decimal price, Lot lot, PlayerStats playerStats, string shopSource, decimal cohort, string region, int selloutPoints)
		: base(playerStats.PaymentCount.Value == 0, price, AnalyticsExtensions.GetItemIdWithPostfix(data, lot), shopSource, cohort, region, selloutPoints)
	{
		((AnalyticsEvent)this).AddEventParams("order_id", (object)data.order_id);
		((AnalyticsEvent)this).AddEventParams("transaction_id", (object)data.transaction_id);
	}

	protected override (int, int) RoundCohortInterval(decimal cohort)
	{
		int num = (int)Mathf.Floor((float)cohort / 10f) * 10;
		return (num, num + 10);
	}
}
