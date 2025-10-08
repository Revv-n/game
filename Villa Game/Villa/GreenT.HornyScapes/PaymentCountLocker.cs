using StripClub.Model;

namespace GreenT.HornyScapes;

public class PaymentCountLocker : EqualityLocker<int>
{
	public PaymentCountLocker(int targetValue, Restriction restrictor)
		: base(targetValue, restrictor)
	{
	}
}
