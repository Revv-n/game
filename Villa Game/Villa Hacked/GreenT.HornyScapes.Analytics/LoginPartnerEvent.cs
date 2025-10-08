namespace GreenT.HornyScapes.Analytics;

public class LoginPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "login";

	public LoginPartnerEvent()
		: base("login")
	{
	}
}
