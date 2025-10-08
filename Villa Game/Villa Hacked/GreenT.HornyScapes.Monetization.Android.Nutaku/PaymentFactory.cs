using Nutaku.Unity;

namespace GreenT.HornyScapes.Monetization.Android.Nutaku;

public class PaymentFactory
{
	private readonly string callbackUrl;

	public PaymentFactory(string callbackUrl)
	{
		this.callbackUrl = callbackUrl;
	}

	public Payment CreatePayment(int monetizationID, string itemName, string itemDescription, string itemImageUrl, int convertedPrice)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		Payment val = new Payment
		{
			userId = SdkPlugin.loginInfo.userId,
			callbackUrl = callbackUrl,
			finishPageUrl = "https://example.com/finish"
		};
		PaymentItem val2 = new PaymentItem();
		val2.itemId = monetizationID.ToString();
		val2.itemName = itemName;
		val2.unitPrice = convertedPrice;
		val2.imageUrl = itemImageUrl;
		val2.description = itemDescription;
		val.paymentItems.Add(val2);
		return val;
	}
}
