using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class InvoicesFilteredRequestNoResponse : HTTPConcretteGetRequestNoRespone<List<PaymentIntentData>>
{
	public InvoicesFilteredRequestNoResponse(string url)
		: base(url)
	{
	}
}
