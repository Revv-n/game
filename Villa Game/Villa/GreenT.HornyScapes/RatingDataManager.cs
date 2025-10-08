using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class RatingDataManager : SimpleManager<RatingData>
{
	public RatingData TryGetRatingData(int eventId, int calendarId, int ratingId)
	{
		return Collection.FirstOrDefault((RatingData data) => data.EventID == eventId && data.CalendarID == calendarId && data.TargetRating.ID == ratingId);
	}

	public override void Add(RatingData entity)
	{
		if (!Collection.Any((RatingData data) => data.EventID == entity.EventID && data.CalendarID == entity.CalendarID && data.TargetRating.ID == entity.TargetRating.ID))
		{
			base.Add(entity);
		}
	}
}
