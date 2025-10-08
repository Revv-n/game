using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class InvoicesRequest : HTTPConcretteGetRequest<Response<PaymentIntentData[]>>
{
	private const string REQUEST_URL = "https://hw.testdlab.com/api/stripe/invoices?player_id={0}";

	public InvoicesRequest(string url)
		: base(url)
	{
	}
}
