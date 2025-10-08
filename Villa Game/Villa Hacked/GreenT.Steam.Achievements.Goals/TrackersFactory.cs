using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Card;
using StripClub.Messenger;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.NewEvent.Data;
using Zenject;

namespace GreenT.Steam.Achievements.Goals;

public class TrackersFactory : IFactory<AchievementIdType, ITrackService>, IFactory, IDisposable
{
	private AchievementService _achievementService;

	private readonly IMessengerManager _messageManager;

	private AchievementProvider _achievementProvider;

	private readonly CardsCollection _cardsCollection;

	private readonly CardsCollectionTracker _cardsCollectionTracker;

	private readonly MergeTrackService _mergeTrackService;

	private readonly EventProvider _eventProvider;

	private readonly LockerFactory _lockerFactory;

	private List<IDisposable> disposables = new List<IDisposable>();

	public TrackersFactory(AchievementService achievementService, IMessengerManager messageManager, AchievementProvider achievementProvider, CardsCollection cardsCollection, CardsCollectionTracker cardsCollectionTracker, MergeTrackService mergeTrackService, EventProvider eventProvider, LockerFactory lockerFactory)
	{
		_achievementService = achievementService;
		_messageManager = messageManager;
		_achievementProvider = achievementProvider;
		_cardsCollection = cardsCollection;
		_cardsCollectionTracker = cardsCollectionTracker;
		_mergeTrackService = mergeTrackService;
		_eventProvider = eventProvider;
		_lockerFactory = lockerFactory;
	}

	public ITrackService Create(AchievementIdType idType)
	{
		AchievementDTO achievement = GetAchievement(idType);
		switch (idType)
		{
		case AchievementIdType.ACH_FINISH_TUTORIAL:
			return CreateTutorialFinishTracker(achievement, "6");
		case AchievementIdType.ACH_FINISH_FIRST_ROOM:
			return CreateRoomTracker(achievement, "104");
		case AchievementIdType.ACH_FINISH_SWIMMING_POOL_ROOM:
			return CreateRoomTracker(achievement, "213");
		case AchievementIdType.ACH_FINISH_LIVING_ROOM:
			return CreateRoomTracker(achievement, "426");
		case AchievementIdType.ACH_FINISH_CASINO_ROOM:
			return CreateRoomTracker(achievement, "548");
		case AchievementIdType.ACH_FINISH_CHILL_ZONE_ROOM:
			return CreateRoomTracker(achievement, "230");
		case AchievementIdType.ACH_FINISH_DANCE_FLOOR_ROOM:
			return CreateRoomTracker(achievement, "223");
		case AchievementIdType.ACH_FINISH_PARTY_ROOM:
			return CreateRoomTracker(achievement, "235");
		case AchievementIdType.ACH_FINISH_BAR_ROOM:
			return CreateRoomTracker(achievement, "529");
		case AchievementIdType.ACH_FINISH_TV_ROOM:
			return CreateRoomTracker(achievement, "513");
		case AchievementIdType.ACH_PROMOTE_ANY_TO_3:
			return CreateAnyPromoteTracker(achievement, 3);
		case AchievementIdType.ACH_PROMOTE_ANY_TO_5:
			return CreateAnyPromoteTracker(achievement, 5);
		case AchievementIdType.ACH_PROMOTE_ANY_TO_10:
			return CreateAnyPromoteTracker(achievement, 10);
		case AchievementIdType.ACH_PROMOTE_ANY_TO_15:
			return CreateAnyPromoteTracker(achievement, 15);
		case AchievementIdType.ACH_PROMOTE_ANY_TO_20:
			return CreateAnyPromoteTracker(achievement, 20);
		case AchievementIdType.ACH_PROMOTE_ANY_TO_25:
			return CreateAnyPromoteTracker(achievement, 25);
		case AchievementIdType.ACH_PROMOTE_ANY_TO_30:
			return CreateAnyPromoteTracker(achievement, 30);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_3:
			return CreateGirlInCollectionTracker(achievement, 3);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_5:
			return CreateGirlInCollectionTracker(achievement, 5);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_10:
			return CreateGirlInCollectionTracker(achievement, 10);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_15:
			return CreateGirlInCollectionTracker(achievement, 15);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_20:
			return CreateGirlInCollectionTracker(achievement, 20);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_25:
			return CreateGirlInCollectionTracker(achievement, 25);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_30:
			return CreateGirlInCollectionTracker(achievement, 30);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_35:
			return CreateGirlInCollectionTracker(achievement, 35);
		case AchievementIdType.ACH_GIRL_IN_COLLECTION_40:
			return CreateGirlInCollectionTracker(achievement, 40);
		case AchievementIdType.ACH_COMPLETE_CHAT_1:
			return CreateChatCompleteTracker(achievement, 1);
		case AchievementIdType.ACH_COMPLETE_CHAT_3:
			return CreateChatCompleteTracker(achievement, 3);
		case AchievementIdType.ACH_COMPLETE_CHAT_5:
			return CreateChatCompleteTracker(achievement, 5);
		case AchievementIdType.ACH_COMPLETE_CHAT_8:
			return CreateChatCompleteTracker(achievement, 8);
		case AchievementIdType.ACH_COMPLETE_CHAT_10:
			return CreateChatCompleteTracker(achievement, 10);
		case AchievementIdType.ACH_COMPLETE_CHAT_15:
			return CreateChatCompleteTracker(achievement, 15);
		case AchievementIdType.ACH_COMPLETE_CHAT_20:
			return CreateChatCompleteTracker(achievement, 20);
		case AchievementIdType.ACH_COMPLETE_CHAT_25:
			return CreateChatCompleteTracker(achievement, 25);
		case AchievementIdType.ACH_COMPLETE_CHAT_30:
			return CreateChatCompleteTracker(achievement, 30);
		case AchievementIdType.ACH_COMPLETE_CHAT_35:
			return CreateChatCompleteTracker(achievement, 35);
		case AchievementIdType.ACH_COMPLETE_CHAT_40:
			return CreateChatCompleteTracker(achievement, 40);
		case AchievementIdType.ACH_MERGE_100:
			return CreateMergeDataForTrackService(achievement, 100);
		case AchievementIdType.ACH_MERGE_300:
			return CreateMergeDataForTrackService(achievement, 300);
		case AchievementIdType.ACH_MERGE_800:
			return CreateMergeDataForTrackService(achievement, 800);
		case AchievementIdType.ACH_MERGE_1000:
			return CreateMergeDataForTrackService(achievement, 1000);
		case AchievementIdType.ACH_MERGE_1500:
			return CreateMergeDataForTrackService(achievement, 1500);
		case AchievementIdType.ACH_MERGE_1800:
			return CreateMergeDataForTrackService(achievement, 1800);
		case AchievementIdType.ACH_MERGE_2000:
			return CreateMergeDataForTrackService(achievement, 2000);
		case AchievementIdType.ACH_MERGE_2100:
			return CreateMergeDataForTrackService(achievement, 2100);
		case AchievementIdType.ACH_MERGE_2300:
			return CreateMergeDataForTrackService(achievement, 2300);
		case AchievementIdType.ACH_MERGE_2500:
			return CreateMergeDataForTrackService(achievement, 2500);
		case AchievementIdType.ACH_MERGE_2800:
			return CreateMergeDataForTrackService(achievement, 2800);
		case AchievementIdType.ACH_MERGE_3000:
			return CreateMergeDataForTrackService(achievement, 3000);
		case AchievementIdType.ACH_COLLECT_COMPANY_DARK:
			return CreateCollectGirlCompanyTrackService(achievement, "1017", "1022", "1030");
		case AchievementIdType.ACH_COLLECT_COMPANY_SCENE_LOVERS:
			return CreateCollectGirlCompanyTrackService(achievement, "1015", "1002", "1007", "1005");
		case AchievementIdType.ACH_COLLECT_COMPANY_INTERNET_STAR:
			return CreateCollectGirlCompanyTrackService(achievement, "1003", "1036", "1004");
		case AchievementIdType.ACH_COLLECT_COMPANY_HOMEMAKERS:
			return CreateCollectGirlCompanyTrackService(achievement, "1011", "1009", "1016");
		case AchievementIdType.ACH_COLLECT_COMPANY_REAL_HERO:
			return CreateCollectGirlCompanyTrackService(achievement, "1053", "1055", "1043", "1044");
		case AchievementIdType.ACH_COLLECT_COMPANY_FAIRY_TALE:
			return CreateCollectGirlCompanyTrackService(achievement, "1059", "1060");
		case AchievementIdType.ACH_COLLECT_COMPANY_ASIA:
			return CreateCollectGirlCompanyTrackService(achievement, "1010", "1063", "1065");
		case AchievementIdType.ACH_COLLECT_COMPANY_SPACE_CONQUEROR:
			return CreateCollectGirlCompanyTrackService(achievement, "1032", "1033", "1035");
		case AchievementIdType.ACH_COLLECT_COMPANY_BAD_GIRLS:
			return CreateCollectGirlCompanyTrackService(achievement, "1031", "1006");
		case AchievementIdType.ACH_COLLECT_COMPANY_SUPERVILLAIN:
			return CreateCollectGirlCompanyTrackService(achievement, "1028", "1054");
		case AchievementIdType.ACH_GET_ALL_EVENT_REWARDS:
		{
			CollectAllEventRewards collectAllEventRewards = new CollectAllEventRewards(_achievementService, achievement, _eventProvider);
			disposables.Add(collectAllEventRewards);
			return collectAllEventRewards;
		}
		default:
			return null;
		}
	}

	private ILocker CreateCompanyGirlLocker(params string[] ids)
	{
		List<ILocker> list = new List<ILocker>();
		foreach (string parameters in ids)
		{
			list.Add(_lockerFactory.Create(UnlockType.OwnedGirl, parameters, LockerSourceType.AchievementGirlTracker));
		}
		return new CompositeLocker(list);
	}

	private ITrackService CreateCollectGirlCompanyTrackService(AchievementDTO achievement, params string[] ids)
	{
		ILocker locker = CreateCompanyGirlLocker(ids);
		CollectGirlCompanyTrackService collectGirlCompanyTrackService = new CollectGirlCompanyTrackService(_achievementService, achievement, locker);
		disposables.Add(collectGirlCompanyTrackService);
		return collectGirlCompanyTrackService;
	}

	private ITrackService CreateMergeDataForTrackService(AchievementDTO achievement, int targetValue)
	{
		_mergeTrackService.Add(achievement, targetValue);
		return null;
	}

	private ITrackService CreateTutorialFinishTracker(AchievementDTO achievement, string value)
	{
		ILocker locker = _lockerFactory.Create(UnlockType.TutorGroup, value, LockerSourceType.TutorialFinishTracker);
		FinishTutorialTrackService finishTutorialTrackService = new FinishTutorialTrackService(_achievementService, achievement, locker);
		disposables.Add(finishTutorialTrackService);
		return finishTutorialTrackService;
	}

	private ITrackService CreateRoomTracker(AchievementDTO achievement, string value)
	{
		ILocker locker = _lockerFactory.Create(UnlockType.StepRewarded, value, LockerSourceType.RoomTracker);
		FinishRoomTrackService finishRoomTrackService = new FinishRoomTrackService(_achievementService, achievement, locker);
		disposables.Add(finishRoomTrackService);
		return finishRoomTrackService;
	}

	private ITrackService CreateChatCompleteTracker(AchievementDTO achievement, int targetValue)
	{
		string statsKey = StatsType.STAT_FINISH_CHAT_ANY_TO.ToString();
		FinishChatTrackService finishChatTrackService = new FinishChatTrackService(_achievementService, _messageManager, achievement, statsKey, targetValue);
		disposables.Add(finishChatTrackService);
		return finishChatTrackService;
	}

	private ITrackService CreateAnyPromoteTracker(AchievementDTO achievement, int targetValue)
	{
		string statsKey = StatsType.STAT_PROMOTE_ANY_TO.ToString();
		PromoteAnyGirlToLvlTrackService promoteAnyGirlToLvlTrackService = new PromoteAnyGirlToLvlTrackService(_achievementService, achievement, statsKey, targetValue, _cardsCollection, _cardsCollectionTracker);
		disposables.Add(promoteAnyGirlToLvlTrackService);
		return promoteAnyGirlToLvlTrackService;
	}

	private ITrackService CreateGirlInCollectionTracker(AchievementDTO achievement, int targetValue)
	{
		string statsKey = StatsType.STAT_GIRL_IN_COLLECTION.ToString();
		CountGirlInCollectionTrackService countGirlInCollectionTrackService = new CountGirlInCollectionTrackService(_achievementService, achievement, statsKey, targetValue, _cardsCollection);
		disposables.Add(countGirlInCollectionTrackService);
		return countGirlInCollectionTrackService;
	}

	private AchievementDTO GetAchievement(AchievementIdType idType)
	{
		return _achievementProvider.Collection.FirstOrDefault((AchievementDTO _ach) => _ach.AchievementIdType == idType);
	}

	public void Dispose()
	{
		foreach (IDisposable disposable in disposables)
		{
			disposable.Dispose();
		}
	}
}
