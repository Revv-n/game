using System;
using System.Linq;
using GreenT.HornyScapes.Card;
using GreenT.Steam.Achievements.Goals.Objectives;
using StripClub.Model.Cards;
using UniRx;

namespace GreenT.Steam.Achievements.Goals;

public class PromoteAnyGirlToLvlTrackService : IntTrackService, IDisposable
{
	private readonly CardsCollection _cardsCollection;

	private readonly CardsCollectionTracker _cardsCollectionTracker;

	private IDisposable anyPromoteStream;

	public PromoteAnyGirlToLvlTrackService(AchievementService achievementService, AchievementDTO achievement, string statsKey, int targetValue, CardsCollection cardsCollection, CardsCollectionTracker cardsCollectionTracker)
		: base(achievementService, achievement, statsKey, targetValue)
	{
		_cardsCollection = cardsCollection;
		_cardsCollectionTracker = cardsCollectionTracker;
	}

	public override void Track()
	{
		anyPromoteStream?.Dispose();
		anyPromoteStream = _cardsCollectionTracker.GetAnyPromoteStream().Debug("GetAnyPromoteStream").TakeWhile((ICard _) => !IsComplete())
			.Subscribe(delegate
			{
				try
				{
					UpdateStats();
				}
				catch (Exception exception)
				{
					exception.LogException();
				}
			});
		if (_cardsCollection.Promote.Values.Any())
		{
			UpdateStats();
		}
	}

	private void UpdateStats()
	{
		int num = _cardsCollection.Promote.Values.Max((IPromote _promote) => _promote.Level.Value);
		AchievementService.SetStat(base.StatsKey, num);
		if (num >= base.TargetValue)
		{
			AchievementService.UnlockAchievement(Achievement);
		}
		AchievementService.UpdateStats();
	}

	public void Dispose()
	{
		anyPromoteStream?.Dispose();
	}
}
