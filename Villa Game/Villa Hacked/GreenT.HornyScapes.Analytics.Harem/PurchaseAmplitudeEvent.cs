using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics.Harem;

public class PurchaseAmplitudeEvent : BasePurchaseAmplitudeEvent
{
	public PurchaseAmplitudeEvent(Transaction data, decimal price, Lot lot, PlayerStats playerStats, string shopSource, decimal cohort, string region, int selloutPoints)
		: base(playerStats.PaymentCount.Value == 0, price, AnalyticsExtensions.GetItemIdWithPostfix(data, lot), shopSource, cohort, region, selloutPoints)
	{
		AddBundleKey(data.Id);
	}

	protected override (int, int) RoundCohortInterval(decimal cohort)
	{
		int num = (int)Mathf.Floor((float)cohort / 10f) * 10;
		return (num, num + 10);
	}
}
