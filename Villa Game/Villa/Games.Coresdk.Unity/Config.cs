namespace Games.Coresdk.Unity;

public class Config
{
	public string MerchantId { get; private set; }

	public string ServiceId { get; private set; }

	public string Domain { get; private set; }

	public string ApiDomain { get; private set; }

	public string PaymentDomain { get; private set; }

	public Config(string loginDomain, string paymentDomain)
	{
		Domain = loginDomain;
		ApiDomain = loginDomain + "/api";
		PaymentDomain = paymentDomain;
		MerchantId = string.Empty;
		ServiceId = string.Empty;
	}

	public override string ToString()
	{
		return $"Domain:{Domain}\nApiDomain:{ApiDomain}\nPaymentDomain:{PaymentDomain}";
	}
}
