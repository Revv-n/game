namespace GreenT.HornyScapes.Analytics;

public class NewUserPartnerEvent : PartnerEvent
{
	private const string EVENT_NAME_KEY = "reg";

	public NewUserPartnerEvent()
		: base("reg")
	{
	}
}
