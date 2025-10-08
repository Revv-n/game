using Nutaku.Unity;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class PaymentFactory
{
	private readonly string callbackUrl;

	public PaymentFactory(string callbackUrl)
	{
		this.callbackUrl = callbackUrl;
	}

	public Payment CreatePayment(int monetizationID, string itemName, string itemDescription, string itemImageUrl, int convertedPrice)
	{
		Payment obj = new Payment
		{
			userId = SdkPlugin.loginInfo.userId,
			callbackUrl = callbackUrl,
			finishPageUrl = "https://example.com/finish"
		};
		PaymentItem item = new PaymentItem
		{
			itemId = monetizationID.ToString(),
			itemName = itemName,
			unitPrice = convertedPrice,
			imageUrl = itemImageUrl,
			description = itemDescription
		};
		obj.paymentItems.Add(item);
		return obj;
	}
}
