using System;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

[Serializable]
public class ErolabsOrderData
{
	public string transaction_id;

	public string payment_code;

	public int amount;
}
