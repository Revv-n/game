using GreenT.AssetBundles;
using GreenT.Bonus;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.AntiCheat;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Card;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Collections.Promote.UI;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Dates.Installers;
using GreenT.HornyScapes.Erolabs;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Gallery;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Level;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Messenger;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Meta.Duplicates;
using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Monetization.Data;
using GreenT.HornyScapes.Monetization.Windows.Steam;
using GreenT.HornyScapes.Net;
using GreenT.HornyScapes.Presents;
using GreenT.HornyScapes.Presents.Analytics;
using GreenT.HornyScapes.Presents.Services;
using GreenT.HornyScapes.Relationships;
using GreenT.HornyScapes.Relationships.Analytics;
using GreenT.HornyScapes.Settings;
using GreenT.HornyScapes.StarShop;
using GreenT.HornyScapes.Stories;
using GreenT.HornyScapes.Subscription;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using GreenT.Net;
using GreenT.Settings;
using GreenT.Settings.Data;
using GreenT.Steam;
using Merge;
using ModestTree;
using StripClub;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace GreenT.HornyScapes;

public class GameInstaller : MonoInstaller
{
	[FormerlySerializedAs("AssetBundlesManager")]
	[SerializeField]
	private AssetBundlesService assetBundlesService;

	[SerializeField]
	private ProjectSettings projectSettings;

	[SerializeField]
	private GameSettings gameSettings;

	[SerializeField]
	private CameraChanger cameraChanger;

	[SerializeField]
	private StartFlow startFlow;

	[SerializeField]
	private ScriptableObjectDB localization;

	[SerializeField]
	private LocalizationScreenFactory localizationScreenFactory;

	[SerializeField]
	private LootBoxSettings lootBoxSettings;

	[SerializeField]
	private ErolabsSDKInitializer erolabsSDKInitializer;

	public override void InstallBindings()
	{
		Assert.IsNotNull(projectSettings);
		Assert.IsNotNull(gameSettings);
		BindCoreSystems();
		BindStorage();
		BindPlayersData();
		BindInput();
		BindAssetLoading();
		base.Container.Bind<AssetProvider>().AsSingle();
		base.Container.Bind<BundleLoader>().AsSingle();
		base.Container.Bind<RequestFactory>().AsSingle();
		base.Container.Bind<Wrapper<IAssetBundle>>().WithId("VFX").To<Wrapper<IAssetBundle>>()
			.FromNew()
			.AsCached();
		base.Container.Bind<Wrapper<IAssetBundle>>().WithId("fonts").To<Wrapper<IAssetBundle>>()
			.FromNew()
			.AsCached();
		base.Container.Bind<ScriptableObjectDB>().FromScriptableObject(localization).AsSingle();
		base.Container.Bind<LocalizationScreenFactory>().FromInstance(localizationScreenFactory).AsSingle();
		base.Container.BindInterfacesTo<CameraChanger>().FromInstance(cameraChanger).AsSingle();
		base.Container.Bind<ModifyController>().AsSingle();
		base.Container.Bind<PromoteNotifier>().AsSingle();
		base.Container.Bind<PresentsNotifier>().AsSingle();
		base.Container.Bind<StartFlow>().FromInstance(startFlow);
		base.Container.BindInterfacesAndSelfTo<ConfigFolderLoader>().AsSingle();
		base.Container.Bind<LootBoxSettings>().FromInstance(lootBoxSettings).AsSingle();
		base.Container.BindInterfacesAndSelfTo<MergeIconService>().AsSingle().OnInstantiated<MergeIconService>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
		base.Container.Bind<MainMergeIconProvider>().AsSingle();
		base.Container.Bind<MainMergeIconsLoader>().AsSingle().WithArguments(BundleType.MainFieldItems);
		base.Container.Bind<UIClickSuppressor>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<FrameRateSetter>().AsSingle();
		Installer<TimeInstaller>.Install(base.Container);
		InstallNavigation();
		Installer<AddressablesInstaller>.Install(base.Container);
		Installer<BonusInstaller>.Install(base.Container);
		Installer<BoosterInstaller>.Install(base.Container);
		Installer<SubscriptionCoreInstaller>.Install(base.Container);
		Installer<ContentStorageInstaller>.Install(base.Container);
		Installer<ContentSelectorInstaller>.Install(base.Container);
		Installer<ContentInstaller>.Install(base.Container);
		Installer<MergeInstaller>.Install(base.Container);
		Installer<DataCleanerInstaller>.Install(base.Container);
		Installer<SkinInstaller>.Install(base.Container);
		Installer<BankInstaller>.Install(base.Container);
		Installer<RestoreEnergyInstaller>.Install(base.Container);
		Installer<RestoreEventEnergyInstaller>.Install(base.Container);
		Installer<LockerInstaller>.Install(base.Container);
		Installer<StoryLoaderInstaller>.Install(base.Container);
		Installer<AnalyticsInstaller>.Install(base.Container);
		Installer<CardInstaller>.Install(base.Container);
		Installer<GalleryInstaller>.Install(base.Container);
		Installer<CharacterInstaller>.Install(base.Container);
		Installer<ConstantsInstaller>.Install(base.Container);
		Installer<InputBlockInstaller>.Install(base.Container);
		Installer<LevelUpInstaller>.Install(base.Container);
		Installer<RequestInstaller>.Install(base.Container);
		Installer<LootboxInstaller>.Install(base.Container);
		Installer<StarShopInstaller>.Install(base.Container);
		Installer<MessengerInstaller>.Install(base.Container);
		Installer<DecorationInstaller>.Install(base.Container);
		Installer<StoryInstaller>.Install(base.Container);
		Installer<LocalizationInstaller>.Install(base.Container);
		Installer<DuplicateRewardInstaller>.Install(base.Container);
		Installer<RatingInstaller>.Install(base.Container);
		Installer<LastChanceCommonInstaller>.Install(base.Container);
		Installer<GameItemInstaller>.Install(base.Container);
		Installer<DateInfrastuctureInstaller>.Install(base.Container);
		Installer<RelationshipInstaller>.Install(base.Container);
		Installer<GreenT.HornyScapes.Relationships.Analytics.AnalyticInstaller>.Install(base.Container);
		Installer<PresentsInstaller>.Install(base.Container);
		Installer<GreenT.HornyScapes.Presents.Analytics.AnalyticInstaller>.Install(base.Container);
		Installer<AntiCheatInstaller>.Install(base.Container);
		Installer<SettingsPushInstaller>.Install(base.Container);
		Installer<SteamBridgeInstaller>.Install(base.Container);
		BindMonetizationClasses();
		AddCheats();
	}

	private void BindAssetLoading()
	{
		base.Container.Bind<BundlesProviderBase>().AsSingle();
		base.Container.BindInterfacesTo<AssetBundlesService>().FromInstance(assetBundlesService).AsSingle();
		base.Container.Bind<MainManifestLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AssetBundleLoadMapperManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AssetBundleLoadProcessorManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AssetBundleLoadInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AssetBundleLoadProcessorFactory>().AsSingle();
		base.Container.Bind<StructureInitializerProxyWithArrayFromConfig<AssetBundleLoadMapper>>().AsSingle();
	}

	private void InstallNavigation()
	{
		base.Container.BindInterfacesTo<PCNavigation>().FromNewComponentOn(base.gameObject).AsSingle();
	}

	private void BindMonetizationClasses()
	{
		base.Container.BindInterfacesAndSelfTo<LotOfflineProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<Purchaser>().AsSingle();
		base.Container.Bind<PriceChecker>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizedPriceManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizedPriceInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizedPriceService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<LocalizedPrice>>().AsSingle();
		Installer<MonetizationInstaller>.Install(base.Container);
	}

	private void AddCheats()
	{
	}

	private void BindStorage()
	{
		Bind<PlayerPrefsAdapter>();
		void Bind<T>() where T : IDataStorage
		{
			base.Container.Bind<IDataStorage>().To<T>().AsSingle();
		}
	}

	private void BindCoreSystems()
	{
		base.Container.Bind<GameStopSignal>().AsSingle();
		base.Container.Bind<IProjectSettings>().FromInstance(projectSettings).AsSingle();
		base.Container.Bind<IConfigUrlResolver>().FromResolveGetter((IProjectSettings _settings) => _settings.ConfigUrlResolver).AsSingle();
		base.Container.Bind<IRequestUrlResolver>().FromResolveGetter((IProjectSettings _settings) => _settings.RequestUrlResolver).AsSingle();
		base.Container.Bind<GameSettings>().FromInstance(gameSettings).AsSingle();
		base.Container.Bind<MonoBehaviour>().FromInstance(this).AsSingle();
		base.Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<QueueControllerInitializer>().AsSingle().NonLazy();
		base.Container.BindInterfacesAndSelfTo<RelativeProgress>().AsSingle().WithArguments(0f);
		base.Container.Bind<IGetRequest>().To<HTTPGetRequest>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GetConfigUrlParameters>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GetConfigsVersionRequest>().AsSingle();
	}

	private void BindPlayersData()
	{
		base.Container.Bind<BattlePassLevelProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<User>().AsSingle();
		base.Container.Bind<PlayerExperience>().FromFactory<PlayerExperienceFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PurchaseDataTracker>().AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder concreteIdArgConditionCopyNonLazyBinder = base.Container.Bind<PlayerStats>().AsSingle();
		if (Application.isPlaying)
		{
			concreteIdArgConditionCopyNonLazyBinder.OnInstantiated(delegate(InjectContext _context, PlayerStats _stats)
			{
				_context.Container.Resolve<ISaver>().Add(_stats);
			});
		}
		ConcreteIdArgConditionCopyNonLazyBinder concreteIdArgConditionCopyNonLazyBinder2 = base.Container.Bind<PlayerPaymentsStats>().AsSingle();
		if (Application.isPlaying)
		{
			concreteIdArgConditionCopyNonLazyBinder2.OnInstantiated(delegate(InjectContext _context, PlayerPaymentsStats _stats)
			{
				_context.Container.Resolve<ISaver>().Add(_stats);
			});
		}
		ConcreteIdArgConditionCopyNonLazyBinder concreteIdArgConditionCopyNonLazyBinder3 = base.Container.Bind<GameStartStats>().AsSingle();
		if (Application.isPlaying)
		{
			concreteIdArgConditionCopyNonLazyBinder3.OnInstantiated(delegate(InjectContext _context, GameStartStats _stats)
			{
				_context.Container.Resolve<ISaver>().Add(_stats);
			});
		}
		base.Container.BindInterfacesAndSelfTo<EnergyLoadContainer>().AsSingle();
		base.Container.BindInterfacesTo<PlayersData>().FromFactory<PlayersData, PlayersDataFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PlayerEnergyFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PlayerEventEnergyFactory>().AsSingle();
		base.Container.Bind<CurrenciesService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<Currencies>().AsCached();
		base.Container.BindInterfacesAndSelfTo<SimpleCurrencyFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<TrackableCurrencyActionContainerTracker>().AsSingle();
	}

	private void BindInput()
	{
		base.Container.Bind(typeof(UserInputDetector), typeof(PCUserInputDetector)).To<PCUserInputDetector>().FromNewComponentOn(base.gameObject)
			.AsSingle();
	}
}
