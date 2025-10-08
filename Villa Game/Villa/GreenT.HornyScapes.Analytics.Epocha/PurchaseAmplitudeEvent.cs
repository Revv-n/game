using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics.Epocha;

public class PurchaseAmplitudeEvent : BasePurchaseAmplitudeEvent
{
	private const string BD_ID_KEY = "server_id";

	private const string INVOICE_ID = "invoice_id";

	public PurchaseAmplitudeEvent(PaymentIntentData data, decimal price, Lot lot, PlayerStats playerStats, string shopSource, decimal cohort, string region, int selloutPoints)
		: base(playerStats.PaymentCount.Value == 0, price, AnalyticsExtensions.GetItemIdWithPostfix(data, lot), shopSource, cohort, region, selloutPoints)
	{
		AddBundleKey(data.Bundle);
		AddEventParams("server_id", data.ID);
		AddEventParams("invoice_id", data.InvoiceID);
	}

	protected override (int, int) RoundCohortInterval(decimal cohort)
	{
		int num = (int)Mathf.Floor((float)cohort / 10f) * 10;
		return (num, num + 10);
	}
}
