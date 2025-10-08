using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.BannerSpace;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Dates.Mappers;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Exceptions;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Level;
using GreenT.HornyScapes.Lootboxes.Data;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MergeField;
using GreenT.HornyScapes.MergeStore;
using GreenT.HornyScapes.Messenger;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Meta.Duplicates;
using GreenT.HornyScapes.MiniEvents;
using GreenT.HornyScapes.Presents;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.HornyScapes.StarShop.Data;
using GreenT.HornyScapes.StarShop.Story;
using GreenT.HornyScapes.Stories.Data;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.HornyScapes.Tutorial;
using GreenT.Localizations;
using GreenT.Localizations.Data;
using GreenT.Settings;
using GreenT.Settings.Data;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub;
using StripClub.Gallery.Data;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT;

public class GameController : IDisposable
{
	private readonly GameStarter gameStarter;

	private readonly SignalBus signalBus;

	private readonly IProjectSettings projectSettings;

	private readonly IAssetBundlesLoader bundlesLoader;

	private readonly IAddressablesBundlesLoader addressablesBundlesLoader;

	private readonly Wrapper<IAssetBundle> fontsBundle;

	private readonly Wrapper<IAssetBundle> vfxBundle;

	private readonly ConfigFolderLoader configBatchLoader;

	private readonly IConstants<string> constants;

	private const string BundleVersionKey = "bundles_version";

	private const string BundleBuildKey = "bundles_builld";

	private readonly StructureInitializerProxyWithArrayFromConfig<PhraseMapper> storyStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<LootboxMapper> lootboxInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<ConstantMapper> constantsInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<PromotePatternMapper> promotePatternsInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<MediaMapper> mediaInfoInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<CharacterInfoMapper> characterStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<SkinMapper> skinStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<DuplicateRewardMapper> duplicateRewardsStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<AssetBundleLoadMapper> _assetBundleLoadStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<BoosterMapper> _boosterStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<MergeStoreMapper> _mergeStoreStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<BannerMapper> _bannerStructureInitializer;

	private GameStopSignal _gameStopSignal;

	private readonly StructureInitializerProxyWithArrayFromConfig<DecorationMapper> decorationStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<TaskMapper> taskStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<TaskMapper> eventTaskStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<TaskActivityMapper> miniEventActivityTaskStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<StarShopMapper> starShopStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<StarShopArtMapper> starShopArtInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<EventMapper> eventStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<BattlePassMapper> battlePassStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<MiniEventMapper> miniEventStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<ActivityMapper> miniEventActivityStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<ActivityQuestMapper> miniEventActivityQuestStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<ActivityShopMapper> miniEventActivityShopStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<NoviceCalendarMapper> noviceCalendarStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<PeriodicCalendarMapper> periodicCalendarStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<MiniEventCalendarMapper> miniEventCalendarStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<RatingMapper> ratingStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<PowerMapper> powerStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<MatchmakingMapper> matchmakingStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<TournamentPointsMapper> tournamentPointsStructureInitializer;

	private readonly BankStructureInitializer bankStructureInitializer;

	private readonly BundlesProviderBase bundlesProvider;

	private readonly StructureInitializerProxyWithArrayFromConfig<LocalizedPrice> _localizedPriceStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<GarantChanceMapper> garantChanceStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<RouletteSummonMapper> rouletteSummonStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<RouletteBankSummonMapper> bankRouletteSummonStructureInitializer;

	private readonly MessengerStructureInitializer messengerStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<LocalizationVariantMapper> localizationVariantsStructureInitializer;

	private readonly LanguageSelector languageSelector;

	private readonly MainMergeIconsLoader _mainMergeIconsLoader;

	private readonly MainMergeIconProvider _mainMergeIconProvider;

	private readonly StructureInitializerProxyWithArrayFromConfig<GameItemMapper> _gameItemStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<MergeFieldMapper> _mergeFieldStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<RecipeMapper> _recipeStructureInitializer;

	private readonly GameItemConfigManager gameItemConfigManager;

	private readonly StructureInitializerProxyWithArrayFromConfig<RelationshipRewardMapper> relationshipRewardsStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<RelationshipMapper> relationshipStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<PresentsMapper> presentsStructureInitializer;

	private readonly StructureInitializerProxyWithArrayFromConfig<DatePhraseMapper> datesStructureInitializer;

	private readonly IStructureInitializer<ConfigParser.Folder, RequestType> _selloutStructureInitializer;

	private readonly IStructureInitializer<ConfigParser.Folder, RequestType> _selloutRewardsStructureInitializer;

	private readonly IExceptionHandler exceptionHandler;

	private readonly ReactiveProperty<bool> isGameDataLoaded = new ReactiveProperty<bool>(false);

	private readonly CompositeDisposable streams = new CompositeDisposable();

	private readonly ILoader<IEnumerable<GreenT.HornyScapes.Meta.RoomData>> roomDataLoader;

	private readonly RoomDataManager roomDataManager;

	private readonly Wrapper<IntrinsicRoomConfig> intrinsicRoomConfig;

	private readonly ILoader<IntrinsicRoomConfig> intrinsicLoader;

	private readonly ILoader<IEnumerable<int>, IEnumerable<RoomConfig>> roomConfigsLoader;

	private readonly RoomConfigManager roomConfigManager;

	private readonly BackgroundSpritesCollection backgroundSpritesManager;

	private readonly ILoader<IEnumerable<Sprite>> backgroundLoader;

	private readonly ILoader<IEnumerable<Sprite>> backgroundPreloader;

	private readonly TutorialConfigSO tutorialConfigSo;

	private readonly TutorialInitializer tutorialInitializer;

	private readonly InfoGetItem getItemInfo;

	private readonly RelativeProgress loadingProgress;

	private readonly CompositeDisposable disposables = new CompositeDisposable();

	public IReadOnlyReactiveProperty<bool> IsGameDataLoaded => (IReadOnlyReactiveProperty<bool>)(object)isGameDataLoaded;

	public GameController(GameStarter gameStarter, SignalBus signalBus, IProjectSettings projectSettings, IAssetBundlesLoader bundlesLoader, IAddressablesBundlesLoader addressablesBundlesLoader, [Inject(Id = "fonts")] Wrapper<IAssetBundle> fontsBundle, [Inject(Id = "VFX")] Wrapper<IAssetBundle> vfxBundle, ConfigFolderLoader configBatchLoader, ILoader<IEnumerable<GreenT.HornyScapes.Meta.RoomData>> roomDataLoader, BundlesProviderBase bundlesProvider, StructureInitializerProxyWithArrayFromConfig<PhraseMapper> storyStructureInitializer, StructureInitializerProxyWithArrayFromConfig<LootboxMapper> lootboxInitializer, StructureInitializerProxyWithArrayFromConfig<ConstantMapper> constantsInitializer, StructureInitializerProxyWithArrayFromConfig<PromotePatternMapper> promotePatternsInitializer, StructureInitializerProxyWithArrayFromConfig<MediaMapper> mediaInfoInitializer, StructureInitializerProxyWithArrayFromConfig<CharacterInfoMapper> characterStructureInitializer, StructureInitializerProxyWithArrayFromConfig<LevelsArgsMapper> levelArgsStructureInitializer, [Inject(Id = ContentType.Main)] StructureInitializerProxyWithArrayFromConfig<TaskMapper> taskStructureInitializer, [Inject(Id = ContentType.Event)] StructureInitializerProxyWithArrayFromConfig<TaskMapper> eventTaskStructureInitializer, StructureInitializerProxyWithArrayFromConfig<TaskActivityMapper> miniEventActivityTaskStructureInitializer, StructureInitializerProxyWithArrayFromConfig<StarShopMapper> starShopStructureInitializer, StructureInitializerProxyWithArrayFromConfig<StarShopArtMapper> starShopArtInitializer, StructureInitializerProxyWithArrayFromConfig<EventMapper> eventStructureInitializer, StructureInitializerProxyWithArrayFromConfig<BattlePassMapper> battlePassStructureInitializer, StructureInitializerProxyWithArrayFromConfig<MiniEventMapper> miniEventStructureInitializer, StructureInitializerProxyWithArrayFromConfig<ActivityMapper> miniEventActivityStructureInitializer, StructureInitializerProxyWithArrayFromConfig<ActivityQuestMapper> miniEventActivityQuestStructureInitializer, StructureInitializerProxyWithArrayFromConfig<ActivityShopMapper> miniEventActivityShopStructureInitializer, StructureInitializerProxyWithArrayFromConfig<NoviceCalendarMapper> noviceCalendarStructureInitializer, StructureInitializerProxyWithArrayFromConfig<PeriodicCalendarMapper> periodicCalendarStructureInitializer, StructureInitializerProxyWithArrayFromConfig<DuplicateRewardMapper> duplicateRewardsStructureInitializer, StructureInitializerProxyWithArrayFromConfig<MiniEventCalendarMapper> miniEventCalendarStructureInitializer, StructureInitializerProxyWithArrayFromConfig<GarantChanceMapper> garantChanceStructureInitializer, StructureInitializerProxyWithArrayFromConfig<RouletteBankSummonMapper> bankRouletteSummonStructureInitializer, StructureInitializerProxyWithArrayFromConfig<RouletteSummonMapper> rouletteSummonStructureInitializer, StructureInitializerProxyWithArrayFromConfig<LocalizationVariantMapper> localizationVariantsStructureInitializer, StructureInitializerProxyWithArrayFromConfig<RatingMapper> ratingStructureInitializer, StructureInitializerProxyWithArrayFromConfig<PowerMapper> powerStructureInitializer, StructureInitializerProxyWithArrayFromConfig<TournamentPointsMapper> tournamentPointsStructureInitializer, StructureInitializerProxyWithArrayFromConfig<MatchmakingMapper> matchmakingStructureInitializer, StructureInitializerProxyWithArrayFromConfig<BoosterMapper> boosterStructureInitializer, StructureInitializerProxyWithArrayFromConfig<MergeStoreMapper> mergeStoreStructureInitializer, StructureInitializerProxyWithArrayFromConfig<BannerMapper> bannerStructureInitializer, BankStructureInitializer bankStructureInitializer, MessengerStructureInitializer messengerStructureInitializer, StructureInitializerProxyWithArrayFromConfig<LocalizedPrice> localizedPriceInitializer, RoomDataManager roomDataManager, Wrapper<IntrinsicRoomConfig> intrinsicRoomConfig, ILoader<IntrinsicRoomConfig> intrinsicLoader, ILoader<IEnumerable<int>, IEnumerable<RoomConfig>> roomConfigsLoader, RoomConfigManager roomConfigManager, BackgroundSpritesCollection backgroundSpritesManager, ILoader<IEnumerable<Sprite>> backgroundLoader, [Inject(Id = "Preload")] ILoader<IEnumerable<Sprite>> backgroundPreloader, InfoGetItem getItemInfo, IConstants<string> constants, RelativeProgress loadingProgress, StructureInitializerProxyWithArrayFromConfig<SkinMapper> skinStructureInitializer, IExceptionHandler exceptionHandler, TutorialConfigSO tutorialConfigSo, TutorialInitializer tutorialInitializer, StructureInitializerProxyWithArrayFromConfig<DecorationMapper> decorationStructureInitializer, LanguageSelector languageSelector, StructureInitializerProxyWithArrayFromConfig<GameItemMapper> gameItemStructureInitializer, StructureInitializerProxyWithArrayFromConfig<MergeFieldMapper> mergeFieldStructureInitializer, StructureInitializerProxyWithArrayFromConfig<RecipeMapper> recipeStructureInitializer, GameItemConfigManager gameItemConfigManager, MainMergeIconsLoader mergeIconsLoader, MainMergeIconProvider mergeIconProvider, StructureInitializerProxyWithArrayFromConfig<RelationshipRewardMapper> relationshipRewardsStructureInitializer, StructureInitializerProxyWithArrayFromConfig<RelationshipMapper> relationshipStructureInitializer, StructureInitializerProxyWithArrayFromConfig<DatePhraseMapper> datesStructureInitializer, StructureInitializerProxyWithArrayFromConfig<PresentsMapper> presentsStructureInitializer, StructureInitializerProxyWithArrayFromConfig<SelloutMapper> selloutStructureInitializer, StructureInitializerProxyWithArrayFromConfig<SelloutRewardsMapper> selloutRewardsStructureInitializer, StructureInitializerProxyWithArrayFromConfig<AssetBundleLoadMapper> assetBundleLoadStructureInitializer, GameStopSignal gameStopSignal)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		this.gameStarter = gameStarter;
		this.signalBus = signalBus;
		this.projectSettings = projectSettings;
		this.bundlesLoader = bundlesLoader;
		this.addressablesBundlesLoader = addressablesBundlesLoader;
		this.fontsBundle = fontsBundle;
		this.vfxBundle = vfxBundle;
		this.configBatchLoader = configBatchLoader;
		_gameStopSignal = gameStopSignal;
		_boosterStructureInitializer = boosterStructureInitializer;
		_mergeStoreStructureInitializer = mergeStoreStructureInitializer;
		_bannerStructureInitializer = bannerStructureInitializer;
		this.storyStructureInitializer = storyStructureInitializer;
		this.lootboxInitializer = lootboxInitializer;
		this.constantsInitializer = constantsInitializer;
		this.promotePatternsInitializer = promotePatternsInitializer;
		this.mediaInfoInitializer = mediaInfoInitializer;
		this.characterStructureInitializer = characterStructureInitializer;
		this.taskStructureInitializer = taskStructureInitializer;
		this.eventTaskStructureInitializer = eventTaskStructureInitializer;
		this.miniEventActivityStructureInitializer = miniEventActivityStructureInitializer;
		this.miniEventActivityQuestStructureInitializer = miniEventActivityQuestStructureInitializer;
		this.miniEventActivityShopStructureInitializer = miniEventActivityShopStructureInitializer;
		this.starShopStructureInitializer = starShopStructureInitializer;
		this.starShopArtInitializer = starShopArtInitializer;
		this.eventStructureInitializer = eventStructureInitializer;
		this.battlePassStructureInitializer = battlePassStructureInitializer;
		this.miniEventStructureInitializer = miniEventStructureInitializer;
		this.miniEventActivityTaskStructureInitializer = miniEventActivityTaskStructureInitializer;
		this.noviceCalendarStructureInitializer = noviceCalendarStructureInitializer;
		this.miniEventCalendarStructureInitializer = miniEventCalendarStructureInitializer;
		this.periodicCalendarStructureInitializer = periodicCalendarStructureInitializer;
		this.garantChanceStructureInitializer = garantChanceStructureInitializer;
		this.rouletteSummonStructureInitializer = rouletteSummonStructureInitializer;
		this.bankRouletteSummonStructureInitializer = bankRouletteSummonStructureInitializer;
		this.ratingStructureInitializer = ratingStructureInitializer;
		this.powerStructureInitializer = powerStructureInitializer;
		this.tournamentPointsStructureInitializer = tournamentPointsStructureInitializer;
		this.matchmakingStructureInitializer = matchmakingStructureInitializer;
		this.bankStructureInitializer = bankStructureInitializer;
		_localizedPriceStructureInitializer = localizedPriceInitializer;
		this.messengerStructureInitializer = messengerStructureInitializer;
		this.duplicateRewardsStructureInitializer = duplicateRewardsStructureInitializer;
		this.bundlesProvider = bundlesProvider;
		this.roomDataLoader = roomDataLoader;
		this.roomDataManager = roomDataManager;
		this.intrinsicRoomConfig = intrinsicRoomConfig;
		this.intrinsicLoader = intrinsicLoader;
		this.roomConfigsLoader = roomConfigsLoader;
		this.roomConfigManager = roomConfigManager;
		this.backgroundSpritesManager = backgroundSpritesManager;
		this.backgroundLoader = backgroundLoader;
		this.backgroundPreloader = backgroundPreloader;
		this.getItemInfo = getItemInfo;
		this.constants = constants;
		this.loadingProgress = loadingProgress;
		this.skinStructureInitializer = skinStructureInitializer;
		this.exceptionHandler = exceptionHandler;
		this.tutorialConfigSo = tutorialConfigSo;
		this.tutorialInitializer = tutorialInitializer;
		this.decorationStructureInitializer = decorationStructureInitializer;
		this.localizationVariantsStructureInitializer = localizationVariantsStructureInitializer;
		this.languageSelector = languageSelector;
		_gameItemStructureInitializer = gameItemStructureInitializer;
		_mergeFieldStructureInitializer = mergeFieldStructureInitializer;
		this.gameItemConfigManager = gameItemConfigManager;
		_recipeStructureInitializer = recipeStructureInitializer;
		_mainMergeIconsLoader = mergeIconsLoader;
		_mainMergeIconProvider = mergeIconProvider;
		this.datesStructureInitializer = datesStructureInitializer;
		this.relationshipStructureInitializer = relationshipStructureInitializer;
		this.relationshipRewardsStructureInitializer = relationshipRewardsStructureInitializer;
		this.presentsStructureInitializer = presentsStructureInitializer;
		_selloutStructureInitializer = selloutStructureInitializer;
		_selloutRewardsStructureInitializer = selloutRewardsStructureInitializer;
		_assetBundleLoadStructureInitializer = assetBundleLoadStructureInitializer;
	}

	public void Launch()
	{
		Lot.Set(signalBus);
		IObservable<IAssetBundle> observable = Observable.Do<IAssetBundle>(Observable.ContinueWith<IEnumerable<Sprite>, IAssetBundle>(Observable.Do<IEnumerable<Sprite>>(Observable.ContinueWith<IAssetBundle, IEnumerable<Sprite>>(Observable.Do<IAssetBundle>(Observable.ContinueWith<IAssetBundle, IAssetBundle>(Observable.Do<IAssetBundle>(Observable.ContinueWith<IAssetBundle, IAssetBundle>(Observable.Do<IAssetBundle>(Observable.ContinueWith<Unit, IAssetBundle>(Observable.ContinueWith<Unit, Unit>(Observable.ContinueWith<Unit, Unit>(Observable.TakeUntil<Unit, bool>(CrunchBundlesInitialization(), Observable.Where<bool>((IObservable<bool>)_gameStopSignal.IsStopped, (Func<bool, bool>)((bool x) => x))), (Func<Unit, IObservable<Unit>>)((Unit _) => bundlesLoader.Init())), (Func<Unit, IObservable<Unit>>)((Unit _) => addressablesBundlesLoader.Init())), (Func<Unit, IObservable<IAssetBundle>>)((Unit _) => bundlesLoader.DownloadAssetBundle(projectSettings.BundleUrlResolver.BundleUrl(BundleType.Fonts)))), (Action<IAssetBundle>)fontsBundle.Set), (Func<IAssetBundle, IObservable<IAssetBundle>>)((IAssetBundle _) => bundlesLoader.DownloadAssetBundle(projectSettings.BundleUrlResolver.BundleUrl(BundleType.VFX)))), (Action<IAssetBundle>)vfxBundle.Set), (Func<IAssetBundle, IObservable<IAssetBundle>>)((IAssetBundle _) => bundlesLoader.DownloadAssetBundle(projectSettings.BundleUrlResolver.BundleUrl(BundleType.Currencies)))), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			bundlesProvider.TryAdd(ContentSource.Currencies, bundle);
		}), (Func<IAssetBundle, IObservable<IEnumerable<Sprite>>>)((IAssetBundle _) => _mainMergeIconsLoader.Load())), (Action<IEnumerable<Sprite>>)delegate(IEnumerable<Sprite> x)
		{
			_mainMergeIconProvider.AddRange(x);
		}), (Func<IEnumerable<Sprite>, IObservable<IAssetBundle>>)((IEnumerable<Sprite> _) => bundlesLoader.DownloadAssetBundle(projectSettings.BundleUrlResolver.BundleUrl(BundleType.DateSounds)))), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			bundlesProvider.TryAdd(ContentSource.Default, bundle);
		});
		IObservable<Unit> postLoadedBundles = Observable.AsSingleUnitObservable<IEnumerable<Sprite>>(Observable.Do<IEnumerable<Sprite>>(Observable.ContinueWith<IAssetBundle, IEnumerable<Sprite>>(Observable.Do<IAssetBundle>(Observable.ContinueWith<IAssetBundle, IAssetBundle>(Observable.Do<IAssetBundle>(Observable.Defer<IAssetBundle>((Func<IObservable<IAssetBundle>>)(() => bundlesLoader.DownloadAssetBundle(projectSettings.BundleUrlResolver.BundleUrl(BundleType.OfferBackground)))), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			bundlesProvider.TryAdd(ContentSource.Background, bundle);
		}), (Func<IAssetBundle, IObservable<IAssetBundle>>)((IAssetBundle _) => bundlesLoader.DownloadAssetBundle(projectSettings.BundleUrlResolver.BundleUrl(BundleType.MiniEventBackground)))), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			bundlesProvider.TryAdd(ContentSource.MiniEvent, bundle);
		}), backgroundLoader.Load()), (Action<IEnumerable<Sprite>>)backgroundSpritesManager.AddRange));
		IObservable<IEnumerable<Sprite>> observable2 = Observable.Do<IEnumerable<Sprite>>(Observable.ContinueWith<IEnumerable<RoomConfig>, IEnumerable<Sprite>>(Observable.Do<IEnumerable<RoomConfig>>(Observable.ContinueWith<int[], IEnumerable<RoomConfig>>(Observable.Select<IEnumerable<GreenT.HornyScapes.Meta.RoomData>, int[]>(Observable.Do<IEnumerable<GreenT.HornyScapes.Meta.RoomData>>(Observable.ContinueWith<IntrinsicRoomConfig, IEnumerable<GreenT.HornyScapes.Meta.RoomData>>(Observable.Do<IntrinsicRoomConfig>(Observable.TakeUntil<IntrinsicRoomConfig, bool>(intrinsicLoader.Load(), Observable.Where<bool>((IObservable<bool>)_gameStopSignal.IsStopped, (Func<bool, bool>)((bool x) => x))), (Action<IntrinsicRoomConfig>)intrinsicRoomConfig.Set), roomDataLoader.Load()), (Action<IEnumerable<GreenT.HornyScapes.Meta.RoomData>>)roomDataManager.AddRange), (Func<IEnumerable<GreenT.HornyScapes.Meta.RoomData>, int[]>)((IEnumerable<GreenT.HornyScapes.Meta.RoomData> _roomData) => (from _data in _roomData
			where _data.Preload
			select _data.Id).ToArray())), (Func<int[], IObservable<IEnumerable<RoomConfig>>>)roomConfigsLoader.Load), (Action<IEnumerable<RoomConfig>>)roomConfigManager.AddRange), backgroundPreloader.Load()), (Action<IEnumerable<Sprite>>)backgroundSpritesManager.AddRange);
		ConfigParser.Folder configBatch = null;
		languageSelector.Initialize();
		IConnectableObservable<bool> obj = Observable.Publish<bool>(Observable.ContinueWith<Unit, bool>(Observable.DoOnCompleted<Unit>(Observable.ContinueWith<IEnumerable<Sprite>, Unit>(Observable.DoOnCompleted<IEnumerable<Sprite>>(Observable.ContinueWith<IAssetBundle, IEnumerable<Sprite>>(Observable.ContinueWith<Unit, IAssetBundle>(Observable.ContinueWith<bool, Unit>(Observable.ContinueWith<bool, bool>(Observable.ContinueWith<ConfigParser.Folder, bool>(Observable.Select<ConfigParser.Folder, ConfigParser.Folder>(Observable.TakeUntil<ConfigParser.Folder, bool>(configBatchLoader.Load(), Observable.Where<bool>((IObservable<bool>)_gameStopSignal.IsStopped, (Func<bool, bool>)((bool x) => x))), (Func<ConfigParser.Folder, ConfigParser.Folder>)delegate(ConfigParser.Folder _configBatch)
		{
			configBatch = _configBatch;
			if (configBatch == null)
			{
				Debug.LogError(" Config can't be found. Check your ConnectionSettings", projectSettings.ConfigUrlResolver as ScriptableObject);
			}
			return configBatch;
		}), (Func<ConfigParser.Folder, IObservable<bool>>)((ConfigParser.Folder _) => localizationVariantsStructureInitializer.Initialize(configBatch, RequestType.LocalizationVariants))), (Func<bool, IObservable<bool>>)((bool _) => constantsInitializer.Initialize(configBatch, RequestType.Constants))), SceneLoader.LoadSceneAsObservable(projectSettings.LoginScene)), observable), observable2), (Action)delegate
		{
			loadingProgress.Set(0.92f);
		}), (Func<IEnumerable<Sprite>, IObservable<Unit>>)((IEnumerable<Sprite> _) => InitializeConfigStructures(configBatch))), (Action)delegate
		{
			getItemInfo.Initialize(gameItemConfigManager);
		}), (Func<Unit, IObservable<bool>>)((Unit _) => tutorialInitializer.Initialize(tutorialConfigSo))));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)obj, (Action<bool>)delegate
		{
		}, (Action<Exception>)delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.SendException("GameDataLoading: Necessary data loading");
		}, (Action)OnScenesLoaded), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.SelectMany<bool, Unit>((IObservable<bool>)obj, (Func<bool, IObservable<Unit>>)((bool _) => postLoadedBundles)), (Action<Unit>)delegate
		{
		}, (Action<Exception>)delegate(Exception ex)
		{
			exceptionHandler.Handle(ex);
			throw ex.SendException("GameDataLoading: Post loading");
		}), (ICollection<IDisposable>)streams);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)disposables);
	}

	private IObservable<Unit> CrunchBundlesInitialization()
	{
		IBundleUrlResolver bundleUrlResolver = projectSettings.BundleUrlResolver;
		BundlesRemoteLoadingSettings remoteLoadingSettings = bundleUrlResolver as BundlesRemoteLoadingSettings;
		if ((object)remoteLoadingSettings != null)
		{
			return Observable.Defer<Unit>((Func<IObservable<Unit>>)(() => remoteLoadingSettings.UpdateBundlesRequestUrls(constants["bundles_version"], constants["bundles_builld"])));
		}
		return Observable.ReturnUnit();
	}

	private IObservable<Unit> InitializeConfigStructures(ConfigParser.Folder configBatch)
	{
		return Observable.AsSingleUnitObservable<bool>(Observable.Concat<bool>(promotePatternsInitializer.Initialize(configBatch, RequestType.PromotePatterns), new IObservable<bool>[44]
		{
			_recipeStructureInitializer.Initialize(configBatch, RequestType.Recipes),
			_assetBundleLoadStructureInitializer.Initialize(configBatch, RequestType.AssetLoad),
			_mergeFieldStructureInitializer.Initialize(configBatch, RequestType.MergeField),
			_gameItemStructureInitializer.Initialize(configBatch, RequestType.GameItems),
			characterStructureInitializer.Initialize(configBatch, RequestType.CharactersOverview),
			skinStructureInitializer.Initialize(configBatch, RequestType.CharacterSkin),
			mediaInfoInitializer.Initialize(configBatch, RequestType.MediaInfo),
			lootboxInitializer.Initialize(configBatch, RequestType.Lootboxes),
			_localizedPriceStructureInitializer.Initialize(configBatch, RequestType.RegionPrice),
			bankStructureInitializer.Initialize(configBatch),
			messengerStructureInitializer.Initialize(configBatch),
			taskStructureInitializer.Initialize(configBatch, RequestType.Task),
			eventTaskStructureInitializer.Initialize(configBatch, RequestType.TaskEvent),
			miniEventActivityTaskStructureInitializer.Initialize(configBatch, RequestType.TaskActivities),
			storyStructureInitializer.Initialize(configBatch, RequestType.Story),
			starShopStructureInitializer.Initialize(configBatch, RequestType.StarShop),
			starShopArtInitializer.Initialize(configBatch, RequestType.StarShopArt),
			_boosterStructureInitializer.Initialize(configBatch, RequestType.Booster),
			eventStructureInitializer.Initialize(configBatch, RequestType.EventSettings),
			battlePassStructureInitializer.Initialize(configBatch, RequestType.BattlePass),
			miniEventStructureInitializer.Initialize(configBatch, RequestType.MiniEvents),
			decorationStructureInitializer.Initialize(configBatch, RequestType.Decorations),
			_selloutRewardsStructureInitializer.Initialize(configBatch, RequestType.SelloutRewards),
			_selloutStructureInitializer.Initialize(configBatch, RequestType.Sellout),
			miniEventActivityStructureInitializer.Initialize(configBatch, RequestType.Activities),
			miniEventActivityQuestStructureInitializer.Initialize(configBatch, RequestType.ActivitiesQuest),
			miniEventActivityShopStructureInitializer.Initialize(configBatch, RequestType.ActivitiesShop),
			noviceCalendarStructureInitializer.Initialize(configBatch, RequestType.EventCalendarNovice),
			miniEventCalendarStructureInitializer.Initialize(configBatch, RequestType.MiniEventCalendar),
			periodicCalendarStructureInitializer.Initialize(configBatch, RequestType.EventCalendar),
			duplicateRewardsStructureInitializer.Initialize(configBatch, RequestType.DuplicateRewards),
			garantChanceStructureInitializer.Initialize(configBatch, RequestType.GarantChance),
			rouletteSummonStructureInitializer.Initialize(configBatch, RequestType.MiniRoulette),
			tournamentPointsStructureInitializer.Initialize(configBatch, RequestType.TournamentPoints),
			powerStructureInitializer.Initialize(configBatch, RequestType.RatingPower),
			matchmakingStructureInitializer.Initialize(configBatch, RequestType.MatchmakingPlayer),
			bankRouletteSummonStructureInitializer.Initialize(configBatch, RequestType.Roulette),
			ratingStructureInitializer.Initialize(configBatch, RequestType.Rating),
			_mergeStoreStructureInitializer.Initialize(configBatch, RequestType.ItemsShop),
			_bannerStructureInitializer.Initialize(configBatch, RequestType.Banner),
			datesStructureInitializer.Initialize(configBatch, RequestType.Dates),
			presentsStructureInitializer.Initialize(configBatch, RequestType.Presents),
			relationshipRewardsStructureInitializer.Initialize(configBatch, RequestType.RelationshipRewards),
			relationshipStructureInitializer.Initialize(configBatch, RequestType.Relationship)
		}));
	}

	private void OnScenesLoaded()
	{
		loadingProgress.Set(0.95f);
		gameStarter.SetState(state: true);
	}

	public void Dispose()
	{
		disposables.Dispose();
		isGameDataLoaded?.Dispose();
		streams.Dispose();
	}
}
