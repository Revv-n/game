using GreenT.HornyScapes.Monetization;

namespace GreenT.HornyScapes.Analytics.Steam;

public class PaymentCheckAmplitudeEvent : BasePaymentCheckAmplitudeEvent
{
	public PaymentCheckAmplitudeEvent(Product product, int checkoutAttemptCount)
		: base(product, checkoutAttemptCount)
	{
	}
}
