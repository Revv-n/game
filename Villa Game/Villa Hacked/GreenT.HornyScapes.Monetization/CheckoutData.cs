using System;
using Newtonsoft.Json;

namespace GreenT.HornyScapes.Monetization;

[Serializable]
public struct CheckoutData
{
	public string url;

	[JsonProperty("invoice_id")]
	public string id;
}
