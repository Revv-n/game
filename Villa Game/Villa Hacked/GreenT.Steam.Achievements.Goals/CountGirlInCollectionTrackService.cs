using System;
using System.Linq;
using GreenT.Steam.Achievements.Goals.Objectives;
using GreenT.Types;
using StripClub.Model.Cards;
using UniRx;

namespace GreenT.Steam.Achievements.Goals;

public class CountGirlInCollectionTrackService : IntTrackService, IDisposable
{
	private readonly CardsCollection _cardsCollection;

	private IDisposable stream;

	public CountGirlInCollectionTrackService(AchievementService achievementService, AchievementDTO achievement, string statsKey, int targetValue, CardsCollection cardsCollection)
		: base(achievementService, achievement, statsKey, targetValue)
	{
		_cardsCollection = cardsCollection;
	}

	public override void Track()
	{
		stream?.Dispose();
		stream = ObservableExtensions.Subscribe<ICard>(Observable.TakeWhile<ICard>(Observable.Where<ICard>(_cardsCollection.OnCardUnlock, (Func<ICard, bool>)((ICard _card) => _card.ContentType == ContentType.Main)), (Func<ICard, bool>)((ICard _) => !IsComplete())), (Action<ICard>)delegate
		{
			UpdateStats();
		});
		UpdateStats();
	}

	private void UpdateStats()
	{
		int num = _cardsCollection.Owned.Count();
		AchievementService.SetStat(base.StatsKey, num);
		if (num >= base.TargetValue)
		{
			AchievementService.UnlockAchievement(Achievement);
		}
		AchievementService.UpdateStats();
	}

	public void Dispose()
	{
		stream?.Dispose();
	}
}
