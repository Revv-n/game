using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UniversalAmplitude;

public class AmplitudeEvent
{
	public enum RevenueType
	{
		Purchase,
		Refund
	}

	private Dictionary<string, object> values = new Dictionary<string, object>();

	public AmplitudeEvent(string event_type)
	{
		if (Init())
		{
			AddCoreProperty("event_type", event_type);
		}
	}

	public AmplitudeEvent(string productId, float price, int quantity, RevenueType revenueType)
	{
		if (Init())
		{
			AddCoreProperty("event_type", "purchase");
			AddCoreProperty("productId", productId);
			AddCoreProperty("price", (revenueType == RevenueType.Refund) ? (0f - price) : price);
			AddCoreProperty("quantity", quantity);
			AddCoreProperty("revenueType", revenueType.ToString());
		}
	}

	private bool Init()
	{
		if (!Amplitude.WasInitialized)
		{
			return false;
		}
		AddCoreProperty("device_id", Amplitude.instance.device_id);
		if (Amplitude.instance.user_id.Length > 0)
		{
			AddCoreProperty("user_id", Amplitude.instance.user_id);
		}
		if (Amplitude.instance.ip.Length > 0)
		{
			AddCoreProperty("ip", Amplitude.instance.ip);
		}
		AddCoreProperty("app_version", Amplitude.instance.app_version);
		AddCoreProperty("session_id", Amplitude.instance.sessionid);
		AddCoreProperty("platform", Amplitude.instance.platform);
		AddCoreProperty("time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
		return true;
	}

	public void AddCoreProperty(string key, object value)
	{
		values.Add(key, value);
	}

	public void AddEventProperty(string key, object value)
	{
		if (!values.ContainsKey("event_properties"))
		{
			values.Add("event_properties", new Dictionary<string, object>());
		}
		(values["event_properties"] as Dictionary<string, object>).Add(key, value);
	}

	public void AddUserProperty(string key, object value)
	{
		if (!values.ContainsKey("user_properties"))
		{
			values.Add("user_properties", new Dictionary<string, object>());
		}
		(values["user_properties"] as Dictionary<string, object>).Add(key, value);
	}

	public string ToJSON()
	{
		return ToJSON(values);
	}

	private string ToJSON(Dictionary<string, object> pairs)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		int num = 0;
		foreach (KeyValuePair<string, object> pair in pairs)
		{
			num++;
			stringBuilder.Append("\"" + pair.Key + "\":");
			stringBuilder.Append(ParseElement(pair.Value));
			if (num < pairs.Count)
			{
				stringBuilder.Append(",");
			}
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private string ParseElement(object o)
	{
		string text = "";
		if (o is IList)
		{
			text += "[";
			foreach (object item in o as IList)
			{
				text = text + ParseElement(item) + ",";
			}
			text = text.Substring(0, text.Length - 1);
			return text + "]";
		}
		if (o is Dictionary<string, object>)
		{
			return ToJSON(o as Dictionary<string, object>);
		}
		if (o is float || o is long || o is int)
		{
			return o.ToString();
		}
		return "\"" + o.ToString() + "\"";
	}

	private string InsertID()
	{
		return Guid.NewGuid().ToString();
	}
}
