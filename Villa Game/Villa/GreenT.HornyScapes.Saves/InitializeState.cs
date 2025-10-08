using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Booster.Effect;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Gallery;
using GreenT.HornyScapes.Lockers;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MergeField;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Services;
using GreenT.HornyScapes.StarShop;
using GreenT.HornyScapes.Stories;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tutorial;
using GreenT.HornyScapes.Сrutch;
using StripClub.Messenger;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Saves;

public class InitializeState
{
	private readonly EnergyBonusService _energyBonusService;

	private readonly SpendEventEnergyTracker _spendEventEnergyTracker;

	private readonly SelloutEntryPoint _selloutEntryPoint;

	private readonly RoomConfigController roomConfigController;

	private readonly StarShopManager starShopManager;

	private readonly TaskManagerCluster tasksManagerCluster;

	private readonly LockerController lockerController;

	private readonly LotManager lotManager;

	private readonly PlayerStats playerStats;

	private readonly StoryCluster storyCluster;

	private readonly IPlayerBasics playerBasics;

	private readonly OfferManagerCluster offerManager;

	private readonly GoldenTicketManagerCluster goldenTicketManagerCluster;

	private readonly ContentSelectorGroup contentSelector;

	private readonly ContentStorageController contentStorageController;

	private readonly MergeIconService mergeIconService;

	private readonly TutorialFinisherResolver tutorialFinisherResolver;

	private readonly AnalyticStarter analyticStarter;

	private readonly IOfflineServiceStarter _offlineServiceStarter;

	private readonly CharacterSettingsManager characterSettingsManager;

	private readonly CharacterManagerState _charactersState;

	private readonly SaveModeSelector saveModeSelector;

	private readonly IConstants<int> intConstants;

	private readonly StarResetUponProgress starResetUponProgress;

	private readonly DecorationManager decorationManager;

	private readonly CardsCollection cardsCollection;

	private readonly SkinDataLoadingController skinDataLoadingController;

	private readonly SimpleCurrencyFactory currencyFactory;

	private readonly IGallery gallery;

	private readonly GalleryState galleryState;

	private readonly IMessengerManager messenger;

	private readonly MessengerState messengerState;

	private readonly MergeFieldFactory _fieldFactory;

	private readonly MergeFieldProvider _fieldProvider;

	private readonly MergeFieldManager _fieldManager;

	public InitializeState(RoomConfigController roomConfigController, StarShopManager starShopManager, TaskManagerCluster tasksManagerCluster, LockerController lockerController, LotManager lotManager, PlayerStats playerStats, StoryCluster storyCluster, IPlayerBasics playerBasics, OfferManagerCluster offerManager, GoldenTicketManagerCluster goldenTicketManagerCluster, ContentSelectorGroup contentSelector, ContentStorageController contentStorageController, TutorialFinisherResolver tutorialFinisherResolver, AnalyticStarter analyticStarter, IOfflineServiceStarter offlineServiceStarter, CharacterSettingsManager characterSettingsManager, CharacterManagerState charactersState, SaveModeSelector saveModeSelector, IConstants<int> intConstants, StarResetUponProgress starResetUponProgress, DecorationManager decorationManager, CardsCollection cardsCollection, SimpleCurrencyFactory currencyFactory, IGallery gallery, GalleryState galleryState, IMessengerManager messenger, MessengerState messengerState, MergeFieldProvider fieldProvider, MergeFieldFactory fieldFactory, SelloutEntryPoint selloutEntryPoint, SkinDataLoadingController skinDataLoadingController, MergeIconService mergeIconService, EnergyBonusService energyBonusService, SpendEventEnergyTracker spendEventEnergyTracker, MergeFieldManager fieldManager)
	{
		this.mergeIconService = mergeIconService;
		_energyBonusService = energyBonusService;
		_spendEventEnergyTracker = spendEventEnergyTracker;
		this.roomConfigController = roomConfigController;
		this.starShopManager = starShopManager;
		this.tasksManagerCluster = tasksManagerCluster;
		this.lockerController = lockerController;
		this.lotManager = lotManager;
		this.playerStats = playerStats;
		this.storyCluster = storyCluster;
		this.playerBasics = playerBasics;
		this.offerManager = offerManager;
		this.goldenTicketManagerCluster = goldenTicketManagerCluster;
		this.contentSelector = contentSelector;
		this.contentStorageController = contentStorageController;
		this.tutorialFinisherResolver = tutorialFinisherResolver;
		this.analyticStarter = analyticStarter;
		_offlineServiceStarter = offlineServiceStarter;
		this.characterSettingsManager = characterSettingsManager;
		_charactersState = charactersState;
		this.saveModeSelector = saveModeSelector;
		this.intConstants = intConstants;
		this.starResetUponProgress = starResetUponProgress;
		this.decorationManager = decorationManager;
		this.cardsCollection = cardsCollection;
		this.currencyFactory = currencyFactory;
		this.gallery = gallery;
		this.galleryState = galleryState;
		this.messenger = messenger;
		this.messengerState = messengerState;
		_fieldFactory = fieldFactory;
		_fieldProvider = fieldProvider;
		_fieldManager = fieldManager;
		_selloutEntryPoint = selloutEntryPoint;
		this.skinDataLoadingController = skinDataLoadingController;
	}

	public void InitializeGameStructures()
	{
		contentSelector.Clear();
		contentSelector.Add(contentStorageController);
		contentSelector.Add(mergeIconService);
		InitializeBalance();
		InitializeMergeFieldManager();
		playerBasics.Init();
		storyCluster.Initialize();
		playerStats.Initialize();
		_spendEventEnergyTracker.Initialize();
		_selloutEntryPoint.Initialize();
		starShopManager.Initialize();
		tasksManagerCluster.Initialize();
		cardsCollection.DropPromotes();
		characterSettingsManager.Purge();
		_charactersState.Initialize();
		lotManager.Initialize();
		_offlineServiceStarter.Start();
		contentSelector.Initialize();
		gallery.Purge();
		galleryState.Initialize();
		messenger.Purge();
		messengerState.Initialize();
		tutorialFinisherResolver.Initialize();
		offerManager.Initialize();
		goldenTicketManagerCluster.Initialize();
		skinDataLoadingController.Initialize();
		saveModeSelector.Initialize();
		starResetUponProgress.Initialize();
		decorationManager.Initialize();
		lockerController.Initialize(isGameActive: true);
		roomConfigController.Initialize();
		_energyBonusService.Initialize();
		analyticStarter.Start();
	}

	private void InitializeBalance()
	{
		CreateCurrency(CurrencyType.Soft, "start_soft");
		CreateCurrency(CurrencyType.Hard, "start_hard");
		CreateCurrency(CurrencyType.Energy, "start_energy");
		CreateCurrency(CurrencyType.Star, 0);
		CreateCurrency(CurrencyType.Jewel, 0);
		CreateCurrency(CurrencyType.Contracts, 0);
		CreateCurrency(CurrencyType.BP, 0);
		CreateCurrency(CurrencyType.Event, 0);
		CreateCurrency(CurrencyType.EventXP, 0);
		CreateCurrency(CurrencyType.EventEnergy, "start_event_energy");
		CreateCurrency(CurrencyType.Present1, 0);
		CreateCurrency(CurrencyType.Present2, 0);
		CreateCurrency(CurrencyType.Present3, 0);
		CreateCurrency(CurrencyType.Present4, "start_present_4");
	}

	private void CreateCurrency(CurrencyType type, string key)
	{
		CreateCurrency(type, intConstants[key]);
	}

	private void CreateCurrency(CurrencyType type, int value)
	{
		currencyFactory.Create(type, value);
	}

	private void InitializeMergeFieldManager()
	{
		_fieldProvider.Purge();
		MergeFieldMapper @default = _fieldManager.GetDefault();
		GreenT.HornyScapes.MergeCore.MergeField mergeField = _fieldFactory.Create(@default, 0.ToString());
		_fieldProvider.TryAdd(mergeField);
	}
}
