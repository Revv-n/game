using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class AbortRequest : PostRequest<Response>
{
	public AbortRequest(string url)
		: base(url)
	{
	}
}
