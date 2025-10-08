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
			_lockerManager.OnNew.TakeUntil(_gameStarter.IsGameActive.Where((bool x) => !x)).Subscribe(Track).AddTo(_trackStream);
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
																																		_calendarQueue.OnCalendarStateChange(EventStructureType.Mini).Subscribe(delegate(CalendarModel calendar)
																																		{
																																			TryInitMiniEventInProgressLocker(calendar, miniEventInProgressLocker);
																																		}).AddTo(_trackStream);
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
																																					(_relationshipProvider.Get(relationshipdId) ?? throw new NullReferenceException($"Relationship with id {relationshipdId} not found, locker = {relationshipdId}:{rewardId}")).RewardClaimed.Subscribe(relationshipRewardedLocker.Set).AddTo(_trackStream);
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
																																			_bannerController.BannerNotificationService.OnBannerReady.Subscribe(bannerReadyToShowLocker.Open).AddTo(_trackStream);
																																		}
																																		_bannerController.BannerNotificationService.OnBannerClose.Subscribe(bannerReadyToShowLocker.Close).AddTo(_trackStream);
																																	}
																																}
																																else
																																{
																																	sumPriceAverageLocker.Set(_playerPaymentsStats.GetSumPriceAverage());
																																	_playerPaymentsStats.OnAddNewPaymentUpdate.Subscribe(delegate
																																	{
																																		sumPriceAverageLocker.Set(_playerPaymentsStats.GetSumPriceAverage());
																																	}).AddTo(_trackStream);
																																}
																															}
																															else
																															{
																																(from _signal in _signalBus.GetStream<LotBoughtSignal>()
																																	select _signal.Lot into _lot
																																	where _lot.TabID == lotBoughtBySectionLocker.sectionID
																																	select _lot).Subscribe(lotBoughtBySectionLocker.Add).AddTo(_trackStream);
																															}
																														}
																														else
																														{
																															_playerStats.PaymentCount.Subscribe(paymentCountLocker.Set).AddTo(_trackStream);
																														}
																													}
																													else
																													{
																														(from _signal in _signalBus.GetStream<LotBoughtSignal>()
																															select _signal.Lot).OfType<Lot, ValuableLot<decimal>>().Subscribe(object3.Set).AddTo(_trackStream);
																													}
																												}
																												else
																												{
																													IReadOnlyReactiveProperty<int> countReactiveProperty = currencyProcessor.GetCountReactiveProperty(CurrencyType.Star);
																													starMaxLocker.Set(countReactiveProperty.Value);
																													countReactiveProperty.Subscribe(starMaxLocker.Set).AddTo(_trackStream);
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
																												story.OnUpdate.Subscribe(storyLocker.Set).AddTo(_trackStream);
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
																											tutorialGroupSteps.OnUpdate.Subscribe(tutorialGroupLocker.Set).AddTo(_trackStream);
																										}
																										return;
																									}
																									Dialogue dialogue = _messenger.GetDialogues().FirstOrDefault((Dialogue _dialogue) => _dialogue.ID == dialogueNotCompleteLocker.DialogueID);
																									if (dialogue != null)
																									{
																										dialogueNotCompleteLocker.Set(dialogue);
																										dialogue.OnUpdate.First((Dialogue _dialogue) => _dialogue.IsComplete).Subscribe(dialogueNotCompleteLocker.Set).AddTo(_trackStream);
																									}
																									else
																									{
																										WaitDialogue(dialogueNotCompleteLocker.DialogueID).Subscribe(dialogueNotCompleteLocker.Set).AddTo(_trackStream);
																									}
																									return;
																								}
																								Dialogue dialogue2 = _messenger.GetDialogues().FirstOrDefault((Dialogue _dialogue) => _dialogue.ID == dialogueCompleteLocker.DialogueID);
																								if (dialogue2 != null)
																								{
																									dialogueCompleteLocker.Set(dialogue2);
																									dialogue2.OnUpdate.First((Dialogue _dialogue) => _dialogue.IsComplete).Subscribe(dialogueCompleteLocker.Set).AddTo(_trackStream);
																								}
																								else
																								{
																									WaitDialogue(dialogueCompleteLocker.DialogueID).Subscribe(dialogueCompleteLocker.Set).AddTo(_trackStream);
																								}
																							}
																							else
																							{
																								ICharacter target = _cards.Collection.OfType<ICharacter>().First((ICharacter _card) => _card.ID == promoteLocker.CardID);
																								IObservable<int> observable = null;
																								observable = ((!_cards.Promote.TryGetValue(target, out var value)) ? _cards.OnCardUnlock.Where((ICard _card) => _card == target).Take(1).Select(_cards.GetPromoteOrDefault)
																									.ContinueWith((IPromote _promote) => _promote.Level) : value.Level);
																								observable.Subscribe(delegate(int _level)
																								{
																									promoteLocker.Set(target, _level);
																								}).AddTo(_trackStream);
																							}
																						}
																						else if (_house.IsObjectSet(roomObjectLocker.RoomObjectID))
																						{
																							_house.GetRoomObject(roomObjectLocker.RoomObjectID).OnViewChanged.Subscribe(roomObjectLocker.Set).AddTo(_trackStream);
																						}
																						else
																						{
																							_house.OnNew.Where((Room _) => _house.IsObjectSet(roomObjectLocker.RoomObjectID)).SelectMany((Room _) => _house.GetRoomObject(roomObjectLocker.RoomObjectID).OnViewChanged).Subscribe(roomObjectLocker.Set)
																								.AddTo(_trackStream);
																						}
																					}
																					else
																					{
																						StarShopItem item2 = _starShopManager.GetItem(stepLocker.ItemID);
																						stepLocker.Set(item2);
																						item2.OnUpdate.Subscribe(stepLocker.Set).AddTo(_trackStream);
																					}
																				}
																				else
																				{
																					if (!_tasksManager.TryGetItem(taskLocker.ItemID, out var task))
																					{
																						throw new Exception().SendException($"Doesn't exist task with id = {taskLocker.ItemID}");
																					}
																					taskLocker.Set(task);
																					task.OnUpdate.Subscribe(taskLocker.Set).AddTo(_trackStream);
																				}
																			}
																			else
																			{
																				IEnumerable<ICharacter> ownedCards = _cards.Owned.OfType<ICharacter>();
																				cardLocker.Set(ownedCards);
																				_cards.OnCardUnlock.OfType<ICard, ICharacter>().Subscribe(cardLocker.Set).AddTo(_trackStream);
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
																		(from _signal in _signalBus.GetStream<LotBoughtSignal>()
																			select _signal.Lot into lot
																			where anyLotBoughtLocker.targetID.Any((int target) => target == lot.ID)
																			select lot).Subscribe(anyLotBoughtLocker.Set).AddTo(_trackStream);
																	}
																	else
																	{
																		bool value2 = _subscriptionStorage.Collection.Any((SubscriptionModel item) => item.BaseID == subscriptionActiveLocker.ID);
																		subscriptionActiveLocker.Set(value2);
																		_subscriptionStorage.OnNew.Subscribe(delegate(SubscriptionModel model)
																		{
																			subscriptionActiveLocker.Set(model.BaseID, condition: true);
																		}).AddTo(_trackStream);
																		_subscriptionStorage.OnRemove.Subscribe(delegate(SubscriptionModel model)
																		{
																			subscriptionActiveLocker.Set(model.BaseID, condition: false);
																		}).AddTo(_trackStream);
																	}
																	return;
																}
																Lot lot2 = _lotManager.Collection.FirstOrDefault((Lot _lot) => _lot.ID == lotBoughtLocker.targetID);
																if (lot2 != null)
																{
																	lotBoughtLocker.Set(lot2);
																	(from _signal in _signalBus.GetStream<LotBoughtSignal>()
																		select _signal.Lot into _lot
																		where _lot.ID == lotBoughtLocker.targetID
																		select _lot).Subscribe(lotBoughtLocker.Set).AddTo(_trackStream);
																}
															}
															else
															{
																currencyProcessor.GetCountReactiveProperty(CurrencyType.EventXP).Subscribe(object2.Set);
															}
														}
														else
														{
															currencyProcessor.GetCountReactiveProperty(CurrencyType.EventXP).Subscribe(eventTargetLocker.Set);
														}
														return;
													}
													_calendarQueue.OnCalendarStateChange(EventStructureType.Sellout).Subscribe(delegate(CalendarModel calendar)
													{
														if (_selloutManager.TryGetSellout(calendar.BalanceId, out var sellout))
														{
															bool newIsOpen3 = calendar.CalendarState.Value == EntityStatus.InProgress;
															selloutInProgressLocker.Set(sellout.Bundle, newIsOpen3);
														}
													}).AddTo(_trackStream);
													return;
												}
												_calendarQueue.OnCalendarStateChange(EventStructureType.BattlePass).Subscribe(delegate(CalendarModel calendar)
												{
													if (_battlePassSettingsProvider.TryGetBattlePass(calendar.BalanceId, out var battlePass))
													{
														bool newIsOpen2 = calendar.CalendarState.Value == EntityStatus.InProgress;
														battlePassInProgressLocker.Set(battlePass.Bundle.Type, newIsOpen2);
													}
												}).AddTo(_trackStream);
												return;
											}
											_calendarQueue.OnCalendarStateChange(EventStructureType.Event).Subscribe(delegate(CalendarModel calendar)
											{
												Event event2 = _eventSettingsProvider.GetEvent(calendar.BalanceId);
												if (event2 != null)
												{
													bool newIsOpen = calendar.CalendarState.Value == EntityStatus.InProgress;
													eventInProgressLocker.Set(event2.Bundle.Type, newIsOpen);
												}
											}).AddTo(_trackStream);
											return;
										}
										_calendarQueue.OnCalendarActiveNotNull().Subscribe(delegate(CalendarModel calendar)
										{
											Event @event = _eventSettingsProvider.GetEvent(calendar.BalanceId);
											if (@event != null)
											{
												eventStartedLocker.Set(@event.Bundle.Type);
											}
										}).AddTo(_trackStream);
									}
									else
									{
										TimeSpan timeSpan = TimeSpan.FromSeconds(1.0);
										(from _ in Observable.Interval(timeSpan, Scheduler.MainThreadIgnoreTimeScale).Repeat()
											select _clock.GetTime().ConvertToUnixTimestamp()).Catch(delegate(Exception ex)
										{
											throw ex.LogException();
										}).OnErrorRetry(delegate(Exception ex)
										{
											throw ex.LogException();
										}, timeSpan).Subscribe(intervalLocker.Set)
											.AddTo(_trackStream);
									}
								}
								else
								{
									TimeSpan timeSpan2 = TimeSpan.FromSeconds(1.0);
									_calendarQueue.OnCalendarActiveNotNull(EventStructureType.Event).Do(@object.Set).SelectMany((from _ in Observable.Interval(timeSpan2, Scheduler.MainThreadIgnoreTimeScale).Repeat()
										select _clock.GetTime().ConvertToUnixTimestamp()).Catch(delegate(Exception ex)
									{
										throw ex.LogException();
									}).OnErrorRetry(delegate(Exception ex)
									{
										throw ex.LogException();
									}, timeSpan2))
										.Subscribe(@object.Set)
										.AddTo(_trackStream);
								}
							}
							else
							{
								battlePassLevelProvider.Level.Subscribe(delegate(int level)
								{
									battlePassLevelRangeLocker.Set(level);
								}).AddTo(_trackStream);
							}
						}
						else
						{
							battlePassLevelProvider.Level.Subscribe(battlePassLevelLocker.Set).AddTo(_trackStream);
						}
					}
					else
					{
						(from x in _spendEventEnergyTracker.OnUpdate
							where x.Type.Equals(spendEventEnergyLocker.EventID)
							select x.Count).Subscribe(spendEventEnergyLocker.Set).AddTo(_trackStream);
						_spendEventEnergyTracker.OnReset.Subscribe(delegate
						{
							spendEventEnergyLocker.Reset();
						}).AddTo(_trackStream);
					}
				}
				else
				{
					_mergeNotifier.OnNotify.Subscribe(delegate(GameItem x)
					{
						mergeItemLocker.Set(x.Config.UniqId);
					}).AddTo(_trackStream);
				}
			}
			else
			{
				_summonUseTracker.OnSummon.Subscribe(delegate(SummonUseTracker.Infocase x)
				{
					summonUseLocker.Add(x.AddCount, x.LotId);
				}).AddTo(_trackStream);
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
			(from signal in _signalBus.GetStream<RouletteLotBoughtSignal>()
				select signal.Lot into lot
				where lot.ID == rouletteLotRollLocker.TargetId && lot is RouletteBankSummonLot == isBank
				select lot).Subscribe(rouletteLotRollLocker.Set).AddTo(_trackStream);
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
		return (from _args in _messenger.OnUpdate
			where _args.Dialogue != null
			select _args.Dialogue).First((Dialogue _dialogue) => _dialogue.ID == id).ContinueWith((Dialogue _dialogue) => _dialogue.OnUpdate.First((Dialogue __dialogue) => __dialogue.IsComplete));
	}

	public void Dispose()
	{
		_trackStream?.Dispose();
	}
}
