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
		AddEventParams("calendar_id", calendarId);
		AddEventParams("rating_id", ratingId);
		AddEventParams("place_count", placeCount);
		AddEventParams("group_rating_id", groupRatingId);
	}
}
