using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class ReceivedRequest : PostRequest<Response>
{
	public ReceivedRequest(string url)
		: base(url)
	{
	}
}
