using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Analytics;

public class PartnerEventRequest : PostJsonRequest<Response, IReadOnlyDictionary<string, object>>
{
	public PartnerEventRequest(string requestUrl)
		: base(requestUrl)
	{
	}
}
