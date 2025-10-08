using GreenT.HornyScapes.Analytics;

namespace GreenT.HornyScapes;

public class RatingRewardEvent : AmplitudeEvent
{
	private const string EventTypeKey = "calendar_rating_reward";

	private const string CalendarIdKey = "calendar_id";

	private const string RatingIdKey = "rating_id";

	private const string PlaceCountKey = "place_count";

	private const string GroupRatingIdKey = "group_rating_id";

	public RatingRewardEvent(int calendarId, int ratingId, int placeCount, int groupRatingId)
		: base("calendar_rating_reward")
	{
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("rating_id", (object)ratingId);
		((AnalyticsEvent)this).AddEventParams("place_count", (object)placeCount);
		((AnalyticsEvent)this).AddEventParams("group_rating_id", (object)groupRatingId);
	}
}
