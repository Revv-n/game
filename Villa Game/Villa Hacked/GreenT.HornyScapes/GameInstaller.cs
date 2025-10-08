using System;
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
		Assert.IsNotNull((object)projectSettings);
		Assert.IsNotNull((object)gameSettings);
		BindCoreSystems();
		BindStorage();
		BindPlayersData();
		BindInput();
		BindAssetLoading();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<AssetProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<BundleLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<RequestFactory>()).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<Wrapper<IAssetBundle>>().WithId((object)"VFX").To<Wrapper<IAssetBundle>>()).FromNew().AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<Wrapper<IAssetBundle>>().WithId((object)"fonts").To<Wrapper<IAssetBundle>>()).FromNew().AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<ScriptableObjectDB>()).FromScriptableObject((ScriptableObject)localization).AsSingle();
		((FromBinderGeneric<LocalizationScreenFactory>)(object)((MonoInstallerBase)this).Container.Bind<LocalizationScreenFactory>()).FromInstance(localizationScreenFactory).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesTo<CameraChanger>().FromInstance((object)cameraChanger).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<ModifyController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<PromoteNotifier>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<PresentsNotifier>()).AsSingle();
		((FromBinderGeneric<StartFlow>)(object)((MonoInstallerBase)this).Container.Bind<StartFlow>()).FromInstance(startFlow);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ConfigFolderLoader>()).AsSingle();
		((FromBinderGeneric<LootBoxSettings>)(object)((MonoInstallerBase)this).Container.Bind<LootBoxSettings>()).FromInstance(lootBoxSettings).AsSingle();
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MergeIconService>()).AsSingle()).OnInstantiated<MergeIconService>((Action<InjectContext, MergeIconService>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MainMergeIconProvider>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MainMergeIconsLoader>()).AsSingle()).WithArguments<BundleType>(BundleType.MainFieldItems);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<UIClickSuppressor>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<FrameRateSetter>()).AsSingle();
		Installer<TimeInstaller>.Install(((MonoInstallerBase)this).Container);
		InstallNavigation();
		Installer<AddressablesInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<BonusInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<BoosterInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<SubscriptionCoreInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<ContentStorageInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<ContentSelectorInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<ContentInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<MergeInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<DataCleanerInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<SkinInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<BankInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<RestoreEnergyInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<RestoreEventEnergyInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<LockerInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<StoryLoaderInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<AnalyticsInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<CardInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<GalleryInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<CharacterInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<ConstantsInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<InputBlockInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<LevelUpInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<RequestInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<LootboxInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<StarShopInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<MessengerInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<DecorationInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<StoryInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<LocalizationInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<DuplicateRewardInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<RatingInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<LastChanceCommonInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<GameItemInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<DateInfrastuctureInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<RelationshipInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<GreenT.HornyScapes.Relationships.Analytics.AnalyticInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<PresentsInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<GreenT.HornyScapes.Presents.Analytics.AnalyticInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<AntiCheatInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<SettingsPushInstaller>.Install(((MonoInstallerBase)this).Container);
		Installer<SteamBridgeInstaller>.Install(((MonoInstallerBase)this).Container);
		BindMonetizationClasses();
		AddCheats();
	}

	private void BindAssetLoading()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<BundlesProviderBase>()).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesTo<AssetBundlesService>().FromInstance((object)assetBundlesService).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MainManifestLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<AssetBundleLoadMapperManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<AssetBundleLoadProcessorManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<AssetBundleLoadInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<AssetBundleLoadProcessorFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<StructureInitializerProxyWithArrayFromConfig<AssetBundleLoadMapper>>()).AsSingle();
	}

	private void InstallNavigation()
	{
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<PCNavigation>()).FromNewComponentOn(((Component)this).gameObject).AsSingle();
	}

	private void BindMonetizationClasses()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LotOfflineProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<Purchaser>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<PriceChecker>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LocalizedPriceManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LocalizedPriceInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LocalizedPriceService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<LocalizedPrice>>()).AsSingle();
		Installer<MonetizationInstaller>.Install(((MonoInstallerBase)this).Container);
	}

	private void AddCheats()
	{
	}

	private void BindStorage()
	{
		Bind<PlayerPrefsAdapter>();
		void Bind<T>() where T : IDataStorage
		{
			((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IDataStorage>)(object)((MonoInstallerBase)this).Container.Bind<IDataStorage>()).To<T>()).AsSingle();
		}
	}

	private void BindCoreSystems()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GameStopSignal>()).AsSingle();
		((FromBinderGeneric<IProjectSettings>)(object)((MonoInstallerBase)this).Container.Bind<IProjectSettings>()).FromInstance((IProjectSettings)projectSettings).AsSingle();
		((FromBinderGeneric<IConfigUrlResolver>)(object)((MonoInstallerBase)this).Container.Bind<IConfigUrlResolver>()).FromResolveGetter<IProjectSettings>((Func<IProjectSettings, IConfigUrlResolver>)((IProjectSettings _settings) => _settings.ConfigUrlResolver)).AsSingle();
		((FromBinderGeneric<IRequestUrlResolver>)(object)((MonoInstallerBase)this).Container.Bind<IRequestUrlResolver>()).FromResolveGetter<IProjectSettings>((Func<IProjectSettings, IRequestUrlResolver>)((IProjectSettings _settings) => _settings.RequestUrlResolver)).AsSingle();
		((FromBinderGeneric<GameSettings>)(object)((MonoInstallerBase)this).Container.Bind<GameSettings>()).FromInstance(gameSettings).AsSingle();
		((FromBinderGeneric<MonoBehaviour>)(object)((MonoInstallerBase)this).Container.Bind<MonoBehaviour>()).FromInstance((MonoBehaviour)(object)this).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GameController>()).AsSingle();
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<QueueControllerInitializer>()).AsSingle()).NonLazy();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RelativeProgress>()).AsSingle()).WithArguments<float>(0f);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IGetRequest>)(object)((MonoInstallerBase)this).Container.Bind<IGetRequest>()).To<HTTPGetRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GetConfigUrlParameters>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GetConfigsVersionRequest>()).AsSingle();
	}

	private void BindPlayersData()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<BattlePassLevelProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<User>()).AsSingle();
		((FromBinderGeneric<PlayerExperience>)(object)((MonoInstallerBase)this).Container.Bind<PlayerExperience>()).FromFactory<PlayerExperienceFactory>().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PurchaseDataTracker>()).AsSingle();
		ConcreteIdArgConditionCopyNonLazyBinder val = ((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<PlayerStats>()).AsSingle();
		if (Application.isPlaying)
		{
			((InstantiateCallbackConditionCopyNonLazyBinder)val).OnInstantiated<PlayerStats>((Action<InjectContext, PlayerStats>)delegate(InjectContext _context, PlayerStats _stats)
			{
				_context.Container.Resolve<ISaver>().Add(_stats);
			});
		}
		ConcreteIdArgConditionCopyNonLazyBinder val2 = ((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<PlayerPaymentsStats>()).AsSingle();
		if (Application.isPlaying)
		{
			((InstantiateCallbackConditionCopyNonLazyBinder)val2).OnInstantiated<PlayerPaymentsStats>((Action<InjectContext, PlayerPaymentsStats>)delegate(InjectContext _context, PlayerPaymentsStats _stats)
			{
				_context.Container.Resolve<ISaver>().Add(_stats);
			});
		}
		ConcreteIdArgConditionCopyNonLazyBinder val3 = ((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GameStartStats>()).AsSingle();
		if (Application.isPlaying)
		{
			((InstantiateCallbackConditionCopyNonLazyBinder)val3).OnInstantiated<GameStartStats>((Action<InjectContext, GameStartStats>)delegate(InjectContext _context, GameStartStats _stats)
			{
				_context.Container.Resolve<ISaver>().Add(_stats);
			});
		}
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EnergyLoadContainer>()).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesTo<PlayersData>().FromFactory<PlayersData, PlayersDataFactory>().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PlayerEnergyFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PlayerEventEnergyFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<CurrenciesService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<Currencies>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SimpleCurrencyFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TrackableCurrencyActionContainerTracker>()).AsSingle();
	}

	private void BindInput()
	{
		((FromBinder)((ConcreteBinderNonGeneric)((MonoInstallerBase)this).Container.Bind(new Type[2]
		{
			typeof(UserInputDetector),
			typeof(PCUserInputDetector)
		})).To<PCUserInputDetector>()).FromNewComponentOn(((Component)this).gameObject).AsSingle();
	}
}
