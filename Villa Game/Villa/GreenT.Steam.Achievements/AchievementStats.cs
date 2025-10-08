using System;
using System.Collections.Generic;
using GreenT.Steam.Achievements.Goals;

namespace GreenT.Steam.Achievements;

public class AchievementStats
{
	private readonly TrackersFactory _trackersFactory;

	private readonly MergeTrackService _mergeTrackService;

	public List<ITrackService> TrackServices = new List<ITrackService>();

	public AchievementStats(TrackersFactory trackersFactory, List<ITrackService> singletonTrackServices)
	{
		_trackersFactory = trackersFactory;
		foreach (AchievementIdType value in Enum.GetValues(typeof(AchievementIdType)))
		{
			ITrackService trackService = _trackersFactory.Create(value);
			if (trackService != null)
			{
				TrackServices.Add(trackService);
			}
		}
		TrackServices.AddRange(singletonTrackServices);
	}

	public void Track()
	{
		foreach (ITrackService trackService in TrackServices)
		{
			trackService.Track();
		}
	}
}
