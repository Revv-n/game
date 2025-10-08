using GreenT.Net;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class FailPaymentRequest : HTTPConcretteGetRequest<Response>
{
	public FailPaymentRequest(string url)
		: base(url)
	{
	}
}
