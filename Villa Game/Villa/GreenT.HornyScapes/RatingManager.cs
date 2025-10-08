using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class RatingManager : SimpleManager<Rating>
{
	public Rating GetRatingInfo(int id)
	{
		return collection.FirstOrDefault((Rating _rating) => _rating.ID == id);
	}
}
