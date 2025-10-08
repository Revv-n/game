using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Analytics.Android.Harem;

public class AndroidHaremEventRequest : PostJsonRequest<Response, IReadOnlyDictionary<string, object>>
{
	private const string RequestUrl = "https://event.haremvilla.com/api/event";

	public AndroidHaremEventRequest()
		: base("https://event.haremvilla.com/api/event")
	{
	}
}
