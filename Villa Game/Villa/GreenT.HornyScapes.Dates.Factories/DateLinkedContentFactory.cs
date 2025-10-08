using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Providers;
using GreenT.HornyScapes.Dates.Services;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Dates.Factories;

public class DateLinkedContentFactory : IFactory<int, LinkedContentAnalyticData, LinkedContent, DateLinkedContent>, IFactory
{
	private readonly DateProvider _dateProvider;

	private readonly DateUnlockService _dateUnlockService;

	public DateLinkedContentFactory(DateProvider dateProvider, DateUnlockService dateUnlockService)
	{
		_dateProvider = dateProvider;
		_dateUnlockService = dateUnlockService;
	}

	public DateLinkedContent Create(int dateId, LinkedContentAnalyticData analyticData, LinkedContent nestedContent = null)
	{
		Date date;
		try
		{
			date = _dateProvider.Get(dateId);
		}
		catch
		{
			throw new NullReferenceException($"No date with id: {dateId}");
		}
		return new DateLinkedContent(date, _dateUnlockService, analyticData, nestedContent);
	}
}
