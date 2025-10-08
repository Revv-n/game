using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class SuspendedPaymentsRequest : HTTPConcretteGetRequest<Response<List<PaymentIntentData>>>
{
	public SuspendedPaymentsRequest(string url)
		: base(url)
	{
	}
}
