using GreenT.Data;
using Zenject;

namespace GreenT.HornyScapes;

public class RatingDataFactory : IFactory<int, int, bool, Rating, RatingData>, IFactory
{
	private readonly ISaver _saver;

	private readonly RatingDataManager _ratingDataManager;

	public RatingDataFactory(ISaver saver, RatingDataManager ratingDataManager)
	{
		_saver = saver;
		_ratingDataManager = ratingDataManager;
	}

	public RatingData Create(int eventId, int calendarId, bool isGlobal, Rating rating)
	{
		RatingData ratingData = new RatingData(eventId, calendarId, isGlobal, rating);
		_saver.Add(ratingData);
		_ratingDataManager.Add(ratingData);
		return ratingData;
	}
}
