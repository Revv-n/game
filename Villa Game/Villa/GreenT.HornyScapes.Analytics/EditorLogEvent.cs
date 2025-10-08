namespace GreenT.HornyScapes.Analytics;

public class EditorLogEvent : IEvent
{
	private string message;

	public EditorLogEvent(string message)
	{
		this.message = message;
	}

	public void Send()
	{
	}
}
