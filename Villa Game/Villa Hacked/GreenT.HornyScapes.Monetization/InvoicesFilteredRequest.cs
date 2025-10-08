using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class InvoicesFilteredRequest : HTTPConcretteGetRequest<Response<List<PaymentIntentData>>>
{
	public InvoicesFilteredRequest(string url)
		: base(url)
	{
	}
}
