using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes;

public class RatingAnalytic : BaseAnalytic
{
	public RatingAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
	}

	public void SendRewardReceivedEvent(int calendarId, int ratingId, int placeCount, int groupRatingId)
	{
		RatingRewardEvent analyticsEvent = new RatingRewardEvent(calendarId, ratingId, placeCount, groupRatingId);
		amplitude.AddEvent(analyticsEvent);
	}
}
