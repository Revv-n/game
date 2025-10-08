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
		AddEventCore("productId", productId);
		AddEventCore("price", (revenueType == RevenueType.Refund) ? (-price) : price);
		AddEventCore("quantity", quantity);
		AddEventCore("revenueType", revenueType.ToString());
	}

	private void Init()
	{
		AddEventCore("time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
	}

	public void AddUserProperty(string key, object value)
	{
		if (!values.ContainsKey(userKey))
		{
			values.Add(userKey, new Dictionary<string, object>());
		}
		(values[userKey] as Dictionary<string, object>).Add(key, value);
	}

	private string InsertID()
	{
		return Guid.NewGuid().ToString();
	}
}
