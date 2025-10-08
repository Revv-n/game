using GreenT.Net;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class SuccessPaymentRequest : HTTPConcretteGetRequest<Response>
{
	public SuccessPaymentRequest(string url)
		: base(url)
	{
	}
}
