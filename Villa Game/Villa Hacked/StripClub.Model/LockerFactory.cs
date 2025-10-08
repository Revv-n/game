using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using GreenT.Data;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Lockers;
using GreenT.HornyScapes.Subscription;
using GreenT.HornyScapes.Tutorial;
using Merge.Meta.RoomObjects;
using StripClub.Model.Cards;
using StripClub.Model.Quest;
using StripClub.Model.Shop;
using StripClub.Stories;
using Zenject;

namespace StripClub.Model;

public class LockerFactory : IFactory<UnlockType, string, LockerSourceType, ILocker>, IFactory
{
	private readonly LockerManager lockerManager;

	private readonly ISaver saver;

	private readonly Dictionary<string, ILocker> _lockers = new Dictionary<string, ILocker>();

	public LockerFactory(LockerManager lockerManager, ISaver saver)
	{
		this.lockerManager = lockerManager;
		this.saver = saver;
	}

	public ILocker Create(UnlockType type, string parameters, LockerSourceType sourceType)
	{
		string key = type.ToString() + "_" + parameters;
		if (_lockers.TryGetValue(key, out var value))
		{
			return value;
		}
		value = CreateNew(type, parameters, sourceType);
		_lockers.Add(key, value);
		return value;
	}

	public ILocker CreateNew(UnlockType type, string parameters, LockerSourceType sourceType)
	{
		ILocker locker;
		switch (type)
		{
		case UnlockType.SummonUse:
		{
			string[] array11 = parameters.Split(':', StringSplitOptions.None);
			int targetCount = int.Parse(array11[1]);
			int targetId = int.Parse(array11[0]);
			SummonUseLocker summonUseLocker = new SummonUseLocker(targetCount, targetId);
			saver.Add(summonUseLocker);
			locker = summonUseLocker;
			break;
		}
		case UnlockType.MergeItem:
			locker = new MergeItemLocker(int.Parse(parameters));
			break;
		case UnlockType.SpendEventEnergy:
		{
			string[] array12 = parameters.Split(':', StringSplitOptions.None);
			locker = new SpendEventEnergyLocker(targetValue: int.Parse(array12[1]), eventID: array12[0]);
			break;
		}
		case UnlockType.MinLevel:
		case UnlockType.MaxLevel:
		case UnlockType.Level:
		case UnlockType.BattlePassLevel:
			try
			{
				locker = new BattlePassLevelLocker(int.Parse(parameters), Restriction.Equal);
			}
			catch (Exception innerException19)
			{
				throw new ArgumentException("Can't parse company level from parameters:\"" + parameters + "\"\n", innerException19);
			}
			break;
		case UnlockType.BattlePassLevelRange:
			try
			{
				string[] array13 = parameters.Split(':', StringSplitOptions.None);
				int num9 = int.Parse(array13[0]);
				int num10 = int.Parse(array13[1]);
				locker = new BattlePassLevelRangeLocker(num9, num10);
			}
			catch (Exception innerException18)
			{
				throw innerException18.SendException("EnableFromLocker: Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.BattlePassInProgress:
			locker = new BattlePassInProgressLocker(parameters);
			break;
		case UnlockType.LotBought:
		case UnlockType.LotNotBought:
		{
			int lotID = int.Parse(parameters);
			LotBoughtLocker.Condition condition = ((type != UnlockType.LotBought) ? LotBoughtLocker.Condition.NotBought : LotBoughtLocker.Condition.Bought);
			locker = new LotBoughtLocker(lotID, condition);
			break;
		}
		case UnlockType.AnyLotBought:
		{
			int[] lotID2 = parameters.Split(',', StringSplitOptions.None).Select(int.Parse).ToArray();
			LotBoughtLocker.Condition condition3 = LotBoughtLocker.Condition.Bought;
			locker = new AnyLotBoughtLocker(lotID2, condition3);
			break;
		}
		case UnlockType.SubscriptionActive:
		case UnlockType.SubscriptionNotActive:
		{
			int id = int.Parse(parameters);
			SubscriptionActiveLocker.SubscriptionStatus condition2 = ((type != UnlockType.SubscriptionActive) ? SubscriptionActiveLocker.SubscriptionStatus.NotActive : SubscriptionActiveLocker.SubscriptionStatus.Active);
			locker = new SubscriptionActiveLocker(id, condition2);
			break;
		}
		case UnlockType.NotOwnedGirl:
		case UnlockType.OwnedGirl:
		{
			int cardID2 = int.Parse(parameters);
			Attitude attitude = ((type != UnlockType.OwnedGirl) ? Attitude.NotOwned : Attitude.Owned);
			locker = new CardLocker<ICharacter>(cardID2, attitude);
			break;
		}
		case UnlockType.StepComplete:
			locker = TryCreateStepLocker(EntityStatus.Rewarded, parameters);
			break;
		case UnlockType.StepRewarded:
			locker = TryCreateStepLocker(EntityStatus.Rewarded, parameters);
			break;
		case UnlockType.StepNotRewarded:
			locker = TryCreateStepLocker(~EntityStatus.Rewarded, parameters);
			break;
		case UnlockType.TaskComplete:
			locker = TryCreateTaskLocker(StateType.Complete, parameters);
			break;
		case UnlockType.TaskRewarded:
			locker = TryCreateTaskLocker(StateType.Rewarded, parameters);
			break;
		case UnlockType.TaskNotRewarded:
			locker = TryCreateTaskLocker(~StateType.Rewarded, parameters);
			break;
		case UnlockType.DialogueNotComplete:
			try
			{
				locker = new DialogueNotCompleteLocker(int.Parse(parameters));
			}
			catch (Exception innerException17)
			{
				throw innerException17.SendException("Can't parse dialogue ID from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.DialogueComplete:
			try
			{
				locker = new DialogueCompleteLocker(int.Parse(parameters));
			}
			catch (Exception innerException16)
			{
				throw innerException16.SendException("Can't parse dialogue ID from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.CharacterMinPromote:
		case UnlockType.CharacterMaxPromote:
			try
			{
				string[] array10 = parameters.Split(':', StringSplitOptions.None);
				int cardID = int.Parse(array10[0]);
				int level = int.Parse(array10[1]);
				locker = new PromoteLocker<ICharacter>(cardID, level, type switch
				{
					UnlockType.CharacterMaxPromote => Restriction.Max, 
					UnlockType.CharacterMinPromote => Restriction.Min, 
					_ => Restriction.Equal, 
				});
			}
			catch (Exception innerException15)
			{
				throw innerException15.SendException("Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.EnableFrom:
			try
			{
				string[] array9 = parameters.Split(':', StringSplitOptions.None);
				long from2 = long.Parse(array9[0]);
				long to2 = long.Parse(array9[1]);
				locker = new EnableFromLocker(from2, to2);
			}
			catch (Exception innerException14)
			{
				throw innerException14.SendException("EnableFromLocker: Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.DisableFrom:
			try
			{
				string[] array8 = parameters.Split(':', StringSplitOptions.None);
				int num7 = int.Parse(array8[0]);
				int num8 = int.Parse(array8[1]);
				locker = new DisableFromLocker(num7, num8);
			}
			catch (Exception innerException13)
			{
				throw innerException13.SendException("DisableFrom: Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.EnableFromEvent:
			try
			{
				string[] array7 = parameters.Split(':', StringSplitOptions.None);
				int num5 = int.Parse(array7[0]);
				int num6 = int.Parse(array7[1]);
				locker = new EnableFromEventLocker(num5, num6);
			}
			catch (Exception innerException12)
			{
				throw innerException12.SendException($"{UnlockType.EnableFromEvent}: Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.DisableFromEvent:
			try
			{
				string[] array6 = parameters.Split(':', StringSplitOptions.None);
				int num3 = int.Parse(array6[0]);
				int num4 = int.Parse(array6[1]);
				locker = new DisableFromEventLocker(num3, num4);
			}
			catch (Exception innerException11)
			{
				throw innerException11.SendException($"{UnlockType.DisableFromEvent}: Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.RoomObject:
			try
			{
				string[] array5 = parameters.Split(':', StringSplitOptions.None);
				int roomID = int.Parse(array5[0]);
				int objectID = int.Parse(array5[1]);
				locker = new RoomObjectLocker(roomID, objectID, 1);
			}
			catch (Exception innerException10)
			{
				throw innerException10.SendException("Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.TutorialStep:
			try
			{
				locker = new TutorialStepLocker(int.Parse(parameters));
			}
			catch (Exception innerException9)
			{
				throw innerException9.SendException($"{UnlockType.TutorialStep} Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.TutorGroup:
			try
			{
				locker = new TutorialGroupLocker(int.Parse(parameters));
			}
			catch (Exception innerException8)
			{
				throw innerException8.SendException($"{UnlockType.TutorGroup} Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.StoryComplete:
			try
			{
				locker = new StoryLocker(int.Parse(parameters));
			}
			catch (Exception innerException7)
			{
				throw innerException7.SendException($"{UnlockType.StoryComplete} Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.StarMax:
			try
			{
				locker = new StarMaxLocker(int.Parse(parameters));
			}
			catch (Exception innerException6)
			{
				throw innerException6.SendException($"{UnlockType.StarMax} Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.FirstPurchase:
		{
			FirstPurchaseLocker firstPurchaseLocker = new FirstPurchaseLocker(openOnEvent: false);
			saver.Add(firstPurchaseLocker);
			locker = firstPurchaseLocker;
			break;
		}
		case UnlockType.MinPaymentCount:
		case UnlockType.MaxPaymentCount:
		case UnlockType.PaymentCount:
			try
			{
				locker = new PaymentCountLocker(int.Parse(parameters), type switch
				{
					UnlockType.MaxPaymentCount => Restriction.Max, 
					UnlockType.MinPaymentCount => Restriction.Min, 
					_ => Restriction.Equal, 
				});
			}
			catch (Exception innerException5)
			{
				throw innerException5.SendException("Can't parse payment count from parameters:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.MinBoughtInSection:
		case UnlockType.MaxBoughtInSection:
		case UnlockType.BoughtInSection:
		{
			int sectionID;
			int count;
			Restriction restrictor;
			try
			{
				string[] array4 = parameters.Split(':', StringSplitOptions.None);
				sectionID = int.Parse(array4[0]);
				count = int.Parse(array4[1]);
				restrictor = type switch
				{
					UnlockType.MaxBoughtInSection => Restriction.Max, 
					UnlockType.MinBoughtInSection => Restriction.Min, 
					_ => Restriction.Equal, 
				};
			}
			catch (Exception innerException4)
			{
				throw innerException4.SendException("Can't parse parameters:\"" + parameters + "\"\n. Parameters must be set as int:int\n");
			}
			LotBoughtBySectionLocker lotBoughtBySectionLocker = new LotBoughtBySectionLocker(sectionID, count, restrictor);
			saver.Add(lotBoughtBySectionLocker);
			locker = lotBoughtBySectionLocker;
			break;
		}
		case UnlockType.EventStart:
			locker = new EventStartedLocker(parameters);
			break;
		case UnlockType.EventInProgress:
			locker = new EventInProgressLocker(parameters);
			break;
		case UnlockType.EventTarget:
			locker = new EventTargetLocker(int.Parse(parameters), Restriction.Min);
			break;
		case UnlockType.EventTargetRange:
			try
			{
				string[] array3 = parameters.Split(':', StringSplitOptions.None);
				int from = int.Parse(array3[0]);
				int to = int.Parse(array3[1]);
				locker = new EventTargetRangeLocker(from, to);
			}
			catch (Exception innerException3)
			{
				throw innerException3.SendException($"{UnlockType.EventTargetRange}: Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.SumPriceAverage:
			try
			{
				string[] array2 = parameters.Split(':', StringSplitOptions.None);
				int num = int.Parse(array2[0]);
				int num2 = int.Parse(array2[1]);
				locker = new SumPriceAverageLocker(num, num2);
			}
			catch (Exception innerException2)
			{
				throw innerException2.SendException($"{UnlockType.SumPriceAverage}: Can't parse parameters from string:\"" + parameters + "\"\n");
			}
			break;
		case UnlockType.MinieventInProgress:
			locker = new MiniEventInProgressLocker(parameters);
			break;
		case UnlockType.RouletteRollLess:
			locker = TryCreateRouletteLotRollLocker(parameters, Restriction.Min);
			break;
		case UnlockType.RouletteRollGreater:
			locker = TryCreateRouletteLotRollLocker(parameters, Restriction.Max);
			break;
		case UnlockType.RouletteBankRollLess:
			locker = TryCreateRouletteLotRollLocker(parameters, Restriction.Min, isBank: true);
			break;
		case UnlockType.RouletteBankRollGreater:
			locker = TryCreateRouletteLotRollLocker(parameters, Restriction.Max, isBank: true);
			break;
		case UnlockType.BannerReadyToShow:
			locker = new BannerReadyToShowLocker(int.Parse(parameters));
			break;
		case UnlockType.Unreachable:
			locker = new UnreachableLocker();
			break;
		case UnlockType.AlwaysAccessible:
			locker = new AccessibleLocker();
			break;
		case UnlockType.RelationshipRewarded:
			try
			{
				string[] array = parameters.Split(':', StringSplitOptions.None);
				int relationshipdId = int.Parse(array[0]);
				int rewardId = int.Parse(array[1]);
				locker = new RelationshipRewardedLocker(relationshipdId, rewardId);
			}
			catch (Exception innerException)
			{
				throw innerException.SendException("Can't parse parameters of RelationshipRewardedLocker " + parameters + ". Parameters must be int:int (relationshipId:rewardId)");
			}
			break;
		default:
			throw new NotImplementedException("There is no behaviour for this type: " + type);
		}
		locker.Source = sourceType;
		lockerManager.Add(locker);
		return locker;
	}

	private RouletteLotRollLocker TryCreateRouletteLotRollLocker(string parameters, Restriction condition, bool isBank = false)
	{
		try
		{
			string[] array = parameters.Split(':', StringSplitOptions.None);
			int targetId = int.Parse(array[0]);
			int rollsRequired = int.Parse(array[1]);
			return isBank ? new RouletteBankLotRollLocker(targetId, rollsRequired, condition) : new RouletteLotRollLocker(targetId, rollsRequired, condition);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("RouletteRollLocker: Can't parse parameters from string:\"" + parameters + "\"\n");
		}
	}

	private StepLocker TryCreateStepLocker(EntityStatus taskStatus, string parameters)
	{
		try
		{
			return new StepLocker(int.Parse(parameters), taskStatus);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't parse task ID from parameters:\"" + parameters + "\"\n");
		}
	}

	private TaskLocker TryCreateTaskLocker(StateType taskStatus, string parameters)
	{
		try
		{
			return new TaskLocker(int.Parse(parameters), taskStatus);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't parse task ID from parameters:\"" + parameters + "\"\n");
		}
	}

	public static CompositeLocker CreateFromParamsArray(UnlockType[] unlock_type, string[] unlock_value, IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, LockerSourceType sourceType)
	{
		ILocker[] array = new ILocker[unlock_type.Length];
		try
		{
			for (int i = 0; i < unlock_type.Length; i++)
			{
				array[i] = lockerFactory.Create(unlock_type[i], unlock_value[i], sourceType);
			}
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Size of array of locker types [{unlock_type.Length}] and parameters [{unlock_value.Length}] must be equal: ");
		}
		return new CompositeLocker(array)
		{
			Source = sourceType
		};
	}
}
