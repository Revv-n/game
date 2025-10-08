namespace GreenT.HornyScapes.Analytics;

public class SignUpPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "signup";

	public SignUpPartnerEvent(string email)
		: base("signup")
	{
		AddEventParams("email", email);
	}
}
