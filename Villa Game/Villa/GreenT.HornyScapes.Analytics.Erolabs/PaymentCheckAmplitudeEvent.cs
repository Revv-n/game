using GreenT.HornyScapes.Monetization;

namespace GreenT.HornyScapes.Analytics.Erolabs;

public class PaymentCheckAmplitudeEvent : BasePaymentCheckAmplitudeEvent
{
	public PaymentCheckAmplitudeEvent(Product product, int checkoutAttemptCount)
		: base(product, checkoutAttemptCount)
	{
	}
}
