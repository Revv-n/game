using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class MarketingEventSender : IMarketingEventSender
{
	private const string PLAY_BUTTON_EVENT = "playBtn";

	public void SendTutorStepEvent(int tutorStepNumber)
	{
		string text = $"tutor_{tutorStepNumber}";
		Application.ExternalCall("SendTrackerEvent", text);
	}

	public async void SendPlayButtonEvent()
	{
		Application.ExternalCall("SendTrackerEvent", "playBtn");
	}
}
