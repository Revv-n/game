using System.Collections.Generic;
using Newtonsoft.Json;

namespace GreenT.HornyScapes.Analytics;

public class PartnerEvent : AnalyticsEvent
{
	public PartnerEvent(string event_type)
		: base("event_name", event_type, "event_params")
	{
		base.values.Add(base.paramsKey, new Dictionary<string, object>());
	}

	public override string ToJSON()
	{
		return JsonConvert.SerializeObject(base.values);
	}
}
