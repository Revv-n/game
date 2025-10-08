using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class RatingControllerManager : SimpleManager<RatingController>
{
	public RatingController TryGetRatingController(RatingData ratingData)
	{
		return Collection.FirstOrDefault((RatingController controller) => controller.EventId == ratingData.EventID && controller.CalendarId == ratingData.CalendarID && controller.RatingId == ratingData.TargetRating.ID);
	}
}
