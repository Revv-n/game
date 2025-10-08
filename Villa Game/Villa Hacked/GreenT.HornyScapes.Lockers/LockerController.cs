using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.BannerSpace;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.MiniEvents;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.Sellouts.Providers;
using GreenT.HornyScapes.StarShop;
using GreenT.HornyScapes.Stories;
using GreenT.HornyScapes.Subscription;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tutorial;
using GreenT.Types;
using Merge;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.Messenger;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using StripClub.NewEvent.Data;
using StripClub.Stories;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Lockers;

public class LockerController : IDisposable
{
	private readonly LockerManager _lockerManager;

	private readonly ICurrencyProcessor currencyProcessor;

	private readonly LotManager _lotManager;

	private readonly CardsCollection _cards;

	private readonly TaskManagerCluster _tasksManager;

	private readonly StarShopManager _starShopManager;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly IDictionary<ContentType, StoryManager> _storyManagerCluster;

	private readonly SignalBus _signalBus;

	private readonly BattlePassLevelProvider battlePassLevelProvider;

	private readonly IMessengerManager _messenger;

	private readonly IClock _clock;

	private readonly TutorialGroupManager _tutorManager;

	private readonly GameStarter _gameStarter;

	private readonly RoomManager _house;

	private readonly PlayerStats _playerStats;

	private readonly CalendarQueue _calendarQueue;

	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly PlayerPaymentsStats _playerPaymentsStats;

	private readonly MiniEventSettingsProvider _miniEventSettingsProvider;

	private readonly MiniEventsBundlesProvider _miniEventsBundlesProvider;

	private readonly RouletteSummonLotManager _rouletteSummonLotManager;

	private readonly RouletteBankSummonLotManager _rouletteBankSummonLotManager;

	private readonly SubscriptionStorage _subscriptionStorage;

	private readonly SubscriptionsActiveRequest _subscriptionsActiveRequest;

	private readonly EventProvider _eventProvider;

	private readonly SummonUseTracker _summonUseTracker;

	private readonly SpendEventEnergyTracker _spendEventEnergyTracker;

	private readonly MergeNotifier _mergeNotifier;

	private readonly BannerController _bannerController;

	private readonly RelationshipProvider _relationshipProvider;

	private readonly SelloutManager _selloutManager;

	private readonly CompositeDisposable _trackStream = new CompositeDisposable();

	public LockerController(LockerManager lockerManager, ICurrencyProcessor currencyProcessor, LotManager lotManager, CardsCollection cards, SignalBus signalBus, BattlePassLevelProvider battlePassLevelProvider, TaskManagerCluster tasksManager, StarShopManager starShopManager, TutorialGroupManager tutorManager, IDictionary<ContentType, StoryManager> storyManagerCluster, IMessengerManager messenger, IClock clock, GameStarter gameStarter, RoomManager house, PlayerStats playerStats, CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider, BattlePassSettingsProvider battlePassSettingsProvider, PlayerPaymentsStats playerPaymentsStats, MiniEventSettingsProvider miniEventSettingsProvider, MiniEventsBundlesProvider miniEventsBundlesProvider, RouletteSummonLotManager rouletteSummonLotManager, SubscriptionStorage subscriptionStorage, SubscriptionsActiveRequest subscriptionsActiveRequest, RouletteBankSummonLotManager rouletteBankSummonLotManager, EventProvider eventProvider, SummonUseTracker summonUseTracker, SpendEventEnergyTracker spendEventEnergyTracker, MergeNotifier mergeNotifier, BannerController bannerController, RelationshipProvider relationshipProvider, SelloutManager selloutManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_lockerManager = lockerManager;
		this.currencyProcessor = currencyProcessor;
		_lotManager = lotManager;
		_cards = cards;
		_signalBus = signalBus;
		this.battlePassLevelProvider = battlePassLevelProvider;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_tasksManager = tasksManager;
		_starShopManager = starShopManager;
		_messenger = messenger;
		_clock = clock;
		_tutorManager = tutorManager;
		_storyManagerCluster = storyManagerCluster;
		_gameStarter = gameStarter;
		_house = house;
		_playerStats = playerStats;
		_calendarQueue = calendarQueue;
		_eventSettingsProvider = eventSettingsProvider;
		_playerPaymentsStats = playerPaymentsStats;
		_miniEventSettingsProvider = miniEventSettingsProvider;
		_miniEventsBundlesProvider = miniEventsBundlesProvider;
		_rouletteSummonLotManager = rouletteSummonLotManager;
		_rouletteBankSummonLotManager = rouletteBankSummonLotManager;
		_subscriptionStorage = subscriptionStorage;
		_subscriptionsActiveRequest = subscriptionsActiveRequest;
		_eventProvider = eventProvider;
		_summonUseTracker = summonUseTracker;
		_spendEventEnergyTracker = spendEventEnergyTracker;
		_mergeNotifier = mergeNotifier;
		_bannerController = bannerController;
		_relationshipProvider = relationshipProvider;
		_selloutManager = selloutManager;
	}

	public void Initialize(bool isGameActive)
	{
		_trackStream.Clear();
		if (isGameActive)
		{
			Locker[] array = _lockerManager.Collection.OfType<Locker>().ToArray();
			foreach (Locker locker in array)
			{
				locker.Initialize();
				Track(locker);
			}
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ILocker>(Observable.TakeUntil<ILocker, bool>(_lockerManager.OnNew, Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => !x))), (Action<ILocker>)Track), (ICollection<IDisposable>)_trackStream);
		}
	}

	private void Track(ILocker locker)
	{
		try
		{
			SummonUseLocker summonUseLocker = locker as SummonUseLocker;
			if (summonUseLocker == null)
			{
				MergeItemLocker mergeItemLocker = locker as MergeItemLocker;
				if (mergeItemLocker == null)
				{
					SpendEventEnergyLocker spendEventEnergyLocker = locker as SpendEventEnergyLocker;
					if (spendEventEnergyLocker == null)
					{
						if (!(locker is BattlePassLevelLocker battlePassLevelLocker))
						{
							BattlePassLevelRangeLocker battlePassLevelRangeLocker = locker as BattlePassLevelRangeLocker;
							if (battlePassLevelRangeLocker == null)
							{
								if (!(locker is EventIntervalLocker @object))
								{
									if (!(locker is IntervalLocker intervalLocker))
									{
										EventStartedLocker eventStartedLocker = locker as EventStartedLocker;
										if (eventStartedLocker == null)
										{
											EventInProgressLocker eventInProgressLocker = locker as EventInProgressLocker;
											if (eventInProgressLocker == null)
											{
												BattlePassInProgressLocker battlePassInProgressLocker = locker as BattlePassInProgressLocker;
												if (battlePassInProgressLocker == null)
												{
													SelloutInProgressLocker selloutInProgressLocker = locker as SelloutInProgressLocker;
													if (selloutInProgressLocker == null)
													{
														if (!(locker is EventTargetLocker eventTargetLocker))
														{
															if (!(locker is EventTargetRangeLocker object2))
															{
																LotBoughtLocker lotBoughtLocker = locker as LotBoughtLocker;
																if (lotBoughtLocker == null)
																{
																	SubscriptionActiveLocker subscriptionActiveLocker = locker as SubscriptionActiveLocker;
																	if (subscriptionActiveLocker == null)
																	{
																		AnyLotBoughtLocker anyLotBoughtLocker = locker as AnyLotBoughtLocker;
																		if (anyLotBoughtLocker == null)
																		{
																			if (!(locker is CardLocker<ICharacter> cardLocker))
																			{
																				if (!(locker is TaskLocker taskLocker))
																				{
																					if (!(locker is StepLocker stepLocker))
																					{
																						RoomObjectLocker roomObjectLocker = locker as RoomObjectLocker;
																						if (roomObjectLocker == null)
																						{
																							PromoteLocker<ICharacter> promoteLocker = locker as PromoteLocker<ICharacter>;
																							if (promoteLocker == null)
																							{
																								DialogueCompleteLocker dialogueCompleteLocker = locker as DialogueCompleteLocker;
																								if (dialogueCompleteLocker == null)
																								{
																									DialogueNotCompleteLocker dialogueNotCompleteLocker = locker as DialogueNotCompleteLocker;
																									if (dialogueNotCompleteLocker == null)
																									{
																										if (!(locker is TutorialGroupLocker tutorialGroupLocker))
																										{
																											if (!(locker is StoryLocker storyLocker))
																											{
																												if (!(locker is StarMaxLocker starMaxLocker))
																												{
																													if (!(locker is FirstPurchaseLocker object3))
																													{
																														if (!(locker is PaymentCountLocker paymentCountLocker))
																														{
																															LotBoughtBySectionLocker lotBoughtBySectionLocker = locker as LotBoughtBySectionLocker;
																															if (lotBoughtBySectionLocker == null)
																															{
																																SumPriceAverageLocker sumPriceAverageLocker = locker as SumPriceAverageLocker;
																																if (sumPriceAverageLocker == null)
																																{
																																	MiniEventInProgressLocker miniEventInProgressLocker = locker as MiniEventInProgressLocker;
																																	if (miniEventInProgressLocker != null)
																																	{
																																		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.Mini), (Action<CalendarModel>)delegate(CalendarModel calendar)
																																		{
																																			TryInitMiniEventInProgressLocker(calendar, miniEventInProgressLocker);
																																		}), (ICollection<IDisposable>)_trackStream);
																																		{
																																			foreach (CalendarModel allActiveCalendar in _calendarQueue.GetAllActiveCalendars(EventStructureType.Mini))
																																			{
																																				TryInitMiniEventInProgressLocker(allActiveCalendar, miniEventInProgressLocker);
																																			}
																																			return;
																																		}
																																	}
																																	if (!(locker is BannerReadyToShowLocker bannerReadyToShowLocker))
																																	{
																																		if (!(locker is RouletteBankLotRollLocker rouletteLotRollLocker))
																																		{
																																			if (!(locker is RouletteLotRollLocker rouletteLotRollLocker2))
																																			{
																																				if (!(locker is UnreachableLocker) && !(locker is AccessibleLocker))
																																				{
																																					if (!(locker is RelationshipRewardedLocker { RelationshipdId: var relationshipdId, RewardId: var rewardId } relationshipRewardedLocker))
																																					{
																																						throw new NotImplementedException("No behavior for this type of locker: " + locker.GetType().ToString());
																																					}
																																					DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((_relationshipProvider.Get(relationshipdId) ?? throw new NullReferenceException($"Relationship with id {relationshipdId} not found, locker = {relationshipdId}:{rewardId}")).RewardClaimed, (Action<int>)relationshipRewardedLocker.Set), (ICollection<IDisposable>)_trackStream);
																																				}
																																			}
																																			else
																																			{
																																				TrackRouletteLotRollLocker(rouletteLotRollLocker2);
																																			}
																																		}
																																		else
																																		{
																																			TrackRouletteLotRollLocker(rouletteLotRollLocker, isBank: true);
																																		}
																																	}
																																	else
																																	{
																																		if (_bannerController.HaveBanner(bannerReadyToShowLocker.ID))
																																		{
																																			bannerReadyToShowLocker.Open(bannerReadyToShowLocker.ID);
																																		}
																																		else
																																		{
																																			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(_bannerController.BannerNotificationService.OnBannerReady, (Action<int>)bannerReadyToShowLocker.Open), (ICollection<IDisposable>)_trackStream);
																																		}
																																		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(_bannerController.BannerNotificationService.OnBannerClose, (Action<int>)bannerReadyToShowLocker.Close), (ICollection<IDisposable>)_trackStream);
																																	}
																																}
																																else
																																{
																																	sumPriceAverageLocker.Set(_playerPaymentsStats.GetSumPriceAverage());
																																	DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<decimal>((IObservable<decimal>)_playerPaymentsStats.OnAddNewPaymentUpdate, (Action<decimal>)delegate
																																	{
																																		sumPriceAverageLocker.Set(_playerPaymentsStats.GetSumPriceAverage());
																																	}), (ICollection<IDisposable>)_trackStream);
																																}
																															}
																															else
																															{
																																DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Lot>(Observable.Where<Lot>(Observable.Select<LotBoughtSignal, Lot>(_signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, Lot>)((LotBoughtSignal _signal) => _signal.Lot)), (Func<Lot, bool>)((Lot _lot) => _lot.TabID == lotBoughtBySectionLocker.sectionID)), (Action<Lot>)lotBoughtBySectionLocker.Add), (ICollection<IDisposable>)_trackStream);
																															}
																														}
																														else
																														{
																															DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)_playerStats.PaymentCount, (Action<int>)paymentCountLocker.Set), (ICollection<IDisposable>)_trackStream);
																														}
																													}
																													else
																													{
																														DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ValuableLot<decimal>>(Observable.OfType<Lot, ValuableLot<decimal>>(Observable.Select<LotBoughtSignal, Lot>(_signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, Lot>)((LotBoughtSignal _signal) => _signal.Lot))), (Action<ValuableLot<decimal>>)object3.Set), (ICollection<IDisposable>)_trackStream);
																													}
																												}
																												else
																												{
																													IReadOnlyReactiveProperty<int> countReactiveProperty = currencyProcessor.GetCountReactiveProperty(CurrencyType.Star);
																													starMaxLocker.Set(countReactiveProperty.Value);
																													DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)countReactiveProperty, (Action<int>)starMaxLocker.Set), (ICollection<IDisposable>)_trackStream);
																												}
																											}
																											else
																											{
																												GreenT.HornyScapes.Stories.Story story = null;
																												IEnumerator<System.Collections.Generic.KeyValuePair<ContentType, StoryManager>> enumerator2 = _storyManagerCluster.GetEnumerator();
																												while (story == null && enumerator2.MoveNext())
																												{
																													story = enumerator2.Current.Value.GetStoryOrDefault(storyLocker.ItemID);
																												}
																												storyLocker.Set(story);
																												DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GreenT.HornyScapes.Stories.Story>(story.OnUpdate, (Action<GreenT.HornyScapes.Stories.Story>)storyLocker.Set), (ICollection<IDisposable>)_trackStream);
																											}
																										}
																										else
																										{
																											TutorialGroupSteps tutorialGroupSteps = null;
																											try
																											{
																												tutorialGroupSteps = _tutorManager.GetGroup(tutorialGroupLocker.GroupID);
																											}
																											catch (Exception innerException)
																											{
																												throw innerException.SendException($"{GetType().Name}: {_tutorManager.GetType().Name} doesn't has GroupID {tutorialGroupLocker.GroupID}");
																											}
																											tutorialGroupLocker.Set(tutorialGroupSteps);
																											DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<TutorialGroupSteps>(tutorialGroupSteps.OnUpdate, (Action<TutorialGroupSteps>)tutorialGroupLocker.Set), (ICollection<IDisposable>)_trackStream);
																										}
																										return;
																									}
																									Dialogue dialogue = _messenger.GetDialogues().FirstOrDefault((Dialogue _dialogue) => _dialogue.ID == dialogueNotCompleteLocker.DialogueID);
																									if (dialogue != null)
																									{
																										dialogueNotCompleteLocker.Set(dialogue);
																										DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Dialogue>(Observable.First<Dialogue>(dialogue.OnUpdate, (Func<Dialogue, bool>)((Dialogue _dialogue) => _dialogue.IsComplete)), (Action<Dialogue>)dialogueNotCompleteLocker.Set), (ICollection<IDisposable>)_trackStream);
																									}
																									else
																									{
																										DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Dialogue>(WaitDialogue(dialogueNotCompleteLocker.DialogueID), (Action<Dialogue>)dialogueNotCompleteLocker.Set), (ICollection<IDisposable>)_trackStream);
																									}
																									return;
																								}
																								Dialogue dialogue2 = _messenger.GetDialogues().FirstOrDefault((Dialogue _dialogue) => _dialogue.ID == dialogueCompleteLocker.DialogueID);
																								if (dialogue2 != null)
																								{
																									dialogueCompleteLocker.Set(dialogue2);
																									DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Dialogue>(Observable.First<Dialogue>(dialogue2.OnUpdate, (Func<Dialogue, bool>)((Dialogue _dialogue) => _dialogue.IsComplete)), (Action<Dialogue>)dialogueCompleteLocker.Set), (ICollection<IDisposable>)_trackStream);
																								}
																								else
																								{
																									DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Dialogue>(WaitDialogue(dialogueCompleteLocker.DialogueID), (Action<Dialogue>)dialogueCompleteLocker.Set), (ICollection<IDisposable>)_trackStream);
																								}
																							}
																							else
																							{
																								ICharacter target = _cards.Collection.OfType<ICharacter>().First((ICharacter _card) => _card.ID == promoteLocker.CardID);
																								IObservable<int> observable = null;
																								observable = ((!_cards.Promote.TryGetValue(target, out var value)) ? Observable.ContinueWith<IPromote, int>(Observable.Select<ICard, IPromote>(Observable.Take<ICard>(Observable.Where<ICard>(_cards.OnCardUnlock, (Func<ICard, bool>)((ICard _card) => _card == target)), 1), (Func<ICard, IPromote>)_cards.GetPromoteOrDefault), (Func<IPromote, IObservable<int>>)((IPromote _promote) => (IObservable<int>)_promote.Level)) : ((IObservable<int>)value.Level));
																								DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(observable, (Action<int>)delegate(int _level)
																								{
																									promoteLocker.Set(target, _level);
																								}), (ICollection<IDisposable>)_trackStream);
																							}
																						}
																						else if (_house.IsObjectSet(roomObjectLocker.RoomObjectID))
																						{
																							DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)_house.GetRoomObject(roomObjectLocker.RoomObjectID).OnViewChanged, (Action<int>)roomObjectLocker.Set), (ICollection<IDisposable>)_trackStream);
																						}
																						else
																						{
																							DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.SelectMany<Room, int>(Observable.Where<Room>(_house.OnNew, (Func<Room, bool>)((Room _) => _house.IsObjectSet(roomObjectLocker.RoomObjectID))), (Func<Room, IObservable<int>>)((Room _) => (IObservable<int>)_house.GetRoomObject(roomObjectLocker.RoomObjectID).OnViewChanged)), (Action<int>)roomObjectLocker.Set), (ICollection<IDisposable>)_trackStream);
																						}
																					}
																					else
																					{
																						StarShopItem item2 = _starShopManager.GetItem(stepLocker.ItemID);
																						stepLocker.Set(item2);
																						DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StarShopItem>(item2.OnUpdate, (Action<StarShopItem>)stepLocker.Set), (ICollection<IDisposable>)_trackStream);
																					}
																				}
																				else
																				{
																					if (!_tasksManager.TryGetItem(taskLocker.ItemID, out var task))
																					{
																						throw new Exception().SendException($"Doesn't exist task with id = {taskLocker.ItemID}");
																					}
																					taskLocker.Set(task);
																					DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(task.OnUpdate, (Action<Task>)taskLocker.Set), (ICollection<IDisposable>)_trackStream);
																				}
																			}
																			else
																			{
																				IEnumerable<ICharacter> ownedCards = _cards.Owned.OfType<ICharacter>();
																				cardLocker.Set(ownedCards);
																				DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ICharacter>(Observable.OfType<ICard, ICharacter>(_cards.OnCardUnlock), (Action<ICharacter>)cardLocker.Set), (ICollection<IDisposable>)_trackStream);
																			}
																			return;
																		}
																		List<Lot> list = _lotManager.Collection.Where((Lot lot) => anyLotBoughtLocker.targetID.Any((int target) => target == lot.ID)).ToList();
																		if (!list.Any())
																		{
																			return;
																		}
																		foreach (Lot item3 in list)
																		{
																			anyLotBoughtLocker.Set(item3);
																		}
																		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Lot>(Observable.Where<Lot>(Observable.Select<LotBoughtSignal, Lot>(_signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, Lot>)((LotBoughtSignal _signal) => _signal.Lot)), (Func<Lot, bool>)((Lot lot) => anyLotBoughtLocker.targetID.Any((int target) => target == lot.ID))), (Action<Lot>)anyLotBoughtLocker.Set), (ICollection<IDisposable>)_trackStream);
																	}
																	else
																	{
																		bool value2 = _subscriptionStorage.Collection.Any((SubscriptionModel item) => item.BaseID == subscriptionActiveLocker.ID);
																		subscriptionActiveLocker.Set(value2);
																		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionModel>(_subscriptionStorage.OnNew, (Action<SubscriptionModel>)delegate(SubscriptionModel model)
																		{
																			subscriptionActiveLocker.Set(model.BaseID, condition: true);
																		}), (ICollection<IDisposable>)_trackStream);
																		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionModel>(_subscriptionStorage.OnRemove, (Action<SubscriptionModel>)delegate(SubscriptionModel model)
																		{
																			subscriptionActiveLocker.Set(model.BaseID, condition: false);
																		}), (ICollection<IDisposable>)_trackStream);
																	}
																	return;
																}
																Lot lot2 = _lotManager.Collection.FirstOrDefault((Lot _lot) => _lot.ID == lotBoughtLocker.targetID);
																if (lot2 != null)
																{
																	lotBoughtLocker.Set(lot2);
																	DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Lot>(Observable.Where<Lot>(Observable.Select<LotBoughtSignal, Lot>(_signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, Lot>)((LotBoughtSignal _signal) => _signal.Lot)), (Func<Lot, bool>)((Lot _lot) => _lot.ID == lotBoughtLocker.targetID)), (Action<Lot>)lotBoughtLocker.Set), (ICollection<IDisposable>)_trackStream);
																}
															}
															else
															{
																ObservableExtensions.Subscribe<int>((IObservable<int>)currencyProcessor.GetCountReactiveProperty(CurrencyType.EventXP), (Action<int>)object2.Set);
															}
														}
														else
														{
															ObservableExtensions.Subscribe<int>((IObservable<int>)currencyProcessor.GetCountReactiveProperty(CurrencyType.EventXP), (Action<int>)eventTargetLocker.Set);
														}
														return;
													}
													DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.Sellout), (Action<CalendarModel>)delegate(CalendarModel calendar)
													{
														if (_selloutManager.TryGetSellout(calendar.BalanceId, out var sellout))
														{
															bool newIsOpen3 = calendar.CalendarState.Value == EntityStatus.InProgress;
															selloutInProgressLocker.Set(sellout.Bundle, newIsOpen3);
														}
													}), (ICollection<IDisposable>)_trackStream);
													return;
												}
												DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.BattlePass), (Action<CalendarModel>)delegate(CalendarModel calendar)
												{
													if (_battlePassSettingsProvider.TryGetBattlePass(calendar.BalanceId, out var battlePass))
													{
														bool newIsOpen2 = calendar.CalendarState.Value == EntityStatus.InProgress;
														battlePassInProgressLocker.Set(battlePass.Bundle.Type, newIsOpen2);
													}
												}), (ICollection<IDisposable>)_trackStream);
												return;
											}
											DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarStateChange(EventStructureType.Event), (Action<CalendarModel>)delegate(CalendarModel calendar)
											{
												Event event2 = _eventSettingsProvider.GetEvent(calendar.BalanceId);
												if (event2 != null)
												{
													bool newIsOpen = calendar.CalendarState.Value == EntityStatus.InProgress;
													eventInProgressLocker.Set(event2.Bundle.Type, newIsOpen);
												}
											}), (ICollection<IDisposable>)_trackStream);
											return;
										}
										DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(), (Action<CalendarModel>)delegate(CalendarModel calendar)
										{
											Event @event = _eventSettingsProvider.GetEvent(calendar.BalanceId);
											if (@event != null)
											{
												eventStartedLocker.Set(@event.Bundle.Type);
											}
										}), (ICollection<IDisposable>)_trackStream);
									}
									else
									{
										TimeSpan timeSpan = TimeSpan.FromSeconds(1.0);
										DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.OnErrorRetry<long, Exception>(Observable.Catch<long, Exception>(Observable.Select<long, long>(Observable.Repeat<long>(Observable.Interval(timeSpan, Scheduler.MainThreadIgnoreTimeScale)), (Func<long, long>)((long _) => _clock.GetTime().ConvertToUnixTimestamp())), (Func<Exception, IObservable<long>>)delegate(Exception ex)
										{
											throw ex.LogException();
										}), (Action<Exception>)delegate(Exception ex)
										{
											throw ex.LogException();
										}, timeSpan), (Action<long>)intervalLocker.Set), (ICollection<IDisposable>)_trackStream);
									}
								}
								else
								{
									TimeSpan timeSpan2 = TimeSpan.FromSeconds(1.0);
									DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.SelectMany<CalendarModel, long>(Observable.Do<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event), (Action<CalendarModel>)@object.Set), Observable.OnErrorRetry<long, Exception>(Observable.Catch<long, Exception>(Observable.Select<long, long>(Observable.Repeat<long>(Observable.Interval(timeSpan2, Scheduler.MainThreadIgnoreTimeScale)), (Func<long, long>)((long _) => _clock.GetTime().ConvertToUnixTimestamp())), (Func<Exception, IObservable<long>>)delegate(Exception ex)
									{
										throw ex.LogException();
									}), (Action<Exception>)delegate(Exception ex)
									{
										throw ex.LogException();
									}, timeSpan2)), (Action<long>)@object.Set), (ICollection<IDisposable>)_trackStream);
								}
							}
							else
							{
								DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)battlePassLevelProvider.Level, (Action<int>)delegate(int level)
								{
									battlePassLevelRangeLocker.Set(level);
								}), (ICollection<IDisposable>)_trackStream);
							}
						}
						else
						{
							DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)battlePassLevelProvider.Level, (Action<int>)battlePassLevelLocker.Set), (ICollection<IDisposable>)_trackStream);
						}
					}
					else
					{
						DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Select<SpendEventEnergyTracker.Data, int>(Observable.Where<SpendEventEnergyTracker.Data>(_spendEventEnergyTracker.OnUpdate, (Func<SpendEventEnergyTracker.Data, bool>)((SpendEventEnergyTracker.Data x) => x.Type.Equals(spendEventEnergyLocker.EventID))), (Func<SpendEventEnergyTracker.Data, int>)((SpendEventEnergyTracker.Data x) => x.Count)), (Action<int>)spendEventEnergyLocker.Set), (ICollection<IDisposable>)_trackStream);
						DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(_spendEventEnergyTracker.OnReset, (Action<Unit>)delegate
						{
							spendEventEnergyLocker.Reset();
						}), (ICollection<IDisposable>)_trackStream);
					}
				}
				else
				{
					DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GameItem>(_mergeNotifier.OnNotify, (Action<GameItem>)delegate(GameItem x)
					{
						mergeItemLocker.Set(x.Config.UniqId);
					}), (ICollection<IDisposable>)_trackStream);
				}
			}
			else
			{
				DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SummonUseTracker.Infocase>(_summonUseTracker.OnSummon, (Action<SummonUseTracker.Infocase>)delegate(SummonUseTracker.Infocase x)
				{
					summonUseLocker.Add(x.AddCount, x.LotId);
				}), (ICollection<IDisposable>)_trackStream);
			}
		}
		catch (Exception innerException2)
		{
			throw innerException2.SendException("Error when trying to track Locker of type: " + locker.GetType()?.ToString() + " with source: " + locker.Source.ToString() + "\n");
		}
	}

	private void TryInitMiniEventInProgressLocker(CalendarModel calendar, MiniEventInProgressLocker miniEventInProgressLocker)
	{
		IAssetBundle assetBundle = _miniEventsBundlesProvider.TryGet(calendar.BalanceId);
		if (assetBundle != null)
		{
			MiniEventBundleData miniEventBundleData = assetBundle.LoadAllAssets<MiniEventBundleData>().FirstOrDefault();
			if (!(miniEventBundleData == null))
			{
				bool newIsOpen = calendar.CalendarState.Value == EntityStatus.InProgress;
				miniEventInProgressLocker.Set(miniEventBundleData.Type, newIsOpen);
			}
		}
	}

	private void TrackRouletteLotRollLocker(RouletteLotRollLocker rouletteLotRollLocker, bool isBank = false)
	{
		RouletteSummonLot rouletteSummonLot = (isBank ? TryGetRouletteBankLot(rouletteLotRollLocker.TargetId) : TryGetRouletteLot(rouletteLotRollLocker.TargetId));
		if (rouletteSummonLot != null)
		{
			rouletteLotRollLocker.Set(rouletteSummonLot);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<RouletteLot>(Observable.Where<RouletteLot>(Observable.Select<RouletteLotBoughtSignal, RouletteLot>(_signalBus.GetStream<RouletteLotBoughtSignal>(), (Func<RouletteLotBoughtSignal, RouletteLot>)((RouletteLotBoughtSignal signal) => signal.Lot)), (Func<RouletteLot, bool>)((RouletteLot lot) => lot.ID == rouletteLotRollLocker.TargetId && lot is RouletteBankSummonLot == isBank)), (Action<RouletteLot>)rouletteLotRollLocker.Set), (ICollection<IDisposable>)_trackStream);
		}
		else
		{
			new Exception().SendException($"Roulette lot with id: {rouletteLotRollLocker.TargetId} does not exist");
		}
	}

	private RouletteSummonLot TryGetRouletteLot(int targetId)
	{
		return _rouletteSummonLotManager.Collection.FirstOrDefault((RouletteSummonLot lot) => lot.ID == targetId);
	}

	private RouletteBankSummonLot TryGetRouletteBankLot(int targetId)
	{
		return _rouletteBankSummonLotManager.Collection.FirstOrDefault((RouletteBankSummonLot lot) => lot.ID == targetId);
	}

	private IObservable<Dialogue> WaitDialogue(int id)
	{
		return Observable.ContinueWith<Dialogue, Dialogue>(Observable.First<Dialogue>(Observable.Select<MessengerUpdateArgs, Dialogue>(Observable.Where<MessengerUpdateArgs>(_messenger.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs _args) => _args.Dialogue != null)), (Func<MessengerUpdateArgs, Dialogue>)((MessengerUpdateArgs _args) => _args.Dialogue)), (Func<Dialogue, bool>)((Dialogue _dialogue) => _dialogue.ID == id)), (Func<Dialogue, IObservable<Dialogue>>)((Dialogue _dialogue) => Observable.First<Dialogue>(_dialogue.OnUpdate, (Func<Dialogue, bool>)((Dialogue __dialogue) => __dialogue.IsComplete))));
	}

	public void Dispose()
	{
		CompositeDisposable trackStream = _trackStream;
		if (trackStream != null)
		{
			trackStream.Dispose();
		}
	}
}
