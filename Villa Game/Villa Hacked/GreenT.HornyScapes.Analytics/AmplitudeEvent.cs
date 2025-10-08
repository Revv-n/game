using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes.Analytics;

public class AmplitudeEvent : AnalyticsEvent
{
	public enum RevenueType
	{
		Purchase,
		Refund
	}

	protected string userKey = "user_properties";

	public AmplitudeEvent(string event_type)
		: base("event_type", event_type, "event_properties")
	{
		Init();
	}

	public AmplitudeEvent(string productId, decimal price, int quantity, RevenueType revenueType)
		: base("event_type", "purchase", "event_properties")
	{
		Init();
		((AnalyticsEvent)this).AddEventCore("productId", (object)productId);
		((AnalyticsEvent)this).AddEventCore("price", (object)((revenueType == RevenueType.Refund) ? (-price) : price));
		((AnalyticsEvent)this).AddEventCore("quantity", (object)quantity);
		((AnalyticsEvent)this).AddEventCore("revenueType", (object)revenueType.ToString());
	}

	private void Init()
	{
		((AnalyticsEvent)this).AddEventCore("time", (object)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
	}

	public void AddUserProperty(string key, object value)
	{
		if (!base.values.ContainsKey(userKey))
		{
			base.values.Add(userKey, new Dictionary<string, object>());
		}
		(base.values[userKey] as Dictionary<string, object>).Add(key, value);
	}

	private string InsertID()
	{
		return Guid.NewGuid().ToString();
	}
}
