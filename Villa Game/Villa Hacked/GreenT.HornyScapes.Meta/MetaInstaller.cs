using System.Collections.Generic;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Meta.Data;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.Settings;
using GreenT.Settings.Data;
using Merge.Meta.RoomObjects;
using Spine.Unity;
using StripClub.Model;
using StripClub.Model.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class MetaInstaller : MonoInstaller<MetaInstaller>
{
	public class HouseConfigLoader : AssetBundleLoader<HouseConfig>
	{
		public HouseConfigLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
			: base(assetBundlesLoader, projectSettings, BundleType.House, "_house")
		{
		}
	}

	public class IntrinsicLoader : AssetBundleLoader<IntrinsicRoomConfig>
	{
		public IntrinsicLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
			: base(assetBundlesLoader, projectSettings, BundleType.Intrinsic, "IntrinsicHouseConfig")
		{
		}
	}

	public RoomObject roomObjectPrefab;

	public CharacterObject spineObjectPrefab;

	public AnimatedRoomObject animatedRoomObjectPrefab;

	public override void InstallBindings()
	{
		BindBundleLoaders();
		BindBackgroundLoad();
		BindRoomsLoad();
		BindFactories();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RoomConfigController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<RoomManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RoomObjectsBuilder>()).AsSingle();
	}

	private void BindBundleLoaders()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<IntrinsicLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<RoomConfigLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IBundlesLoader<int, RoomConfig>>)(object)((MonoInstallerBase)this).Container.Bind<IBundlesLoader<int, RoomConfig>>()).To<RoomConfigProxyLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IBundlesLoader<int, SkeletonAnimation>>)(object)((MonoInstallerBase)this).Container.Bind<IBundlesLoader<int, SkeletonAnimation>>()).To<CharacterSkinLoader>()).AsSingle();
	}

	private void BindBackgroundLoad()
	{
		BundleType bundleType = BundleType.StemMap;
		BundleType bundleType2 = BundleType.StemPreloadedMap;
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<BackgroundSpritesLoader>()).AsCached()).WithArguments<BundleType>(bundleType);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<ILoader<IEnumerable<Sprite>>>().WithId((object)"Preload").To<BackgroundSpritesLoader>()).AsCached()).WithArguments<BundleType>(bundleType2);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BackgroundSpritesCollection>()).AsSingle();
	}

	private void BindRoomsLoad()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<HouseConfigLoader>()).AsSingle();
		((FactoryFromBinder<RoomDataConfig, RoomData>)(object)((MonoInstallerBase)this).Container.BindIFactory<RoomDataConfig, RoomData>()).FromFactory<RoomDataFactory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<RoomDataLoader>()).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<RoomDataManager>()).FromNew().AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<RoomConfigManager>()).FromNew().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<Wrapper<IntrinsicRoomConfig>>()).AsSingle();
	}

	private void BindFactories()
	{
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<RoomObject>)(object)((MonoInstallerBase)this).Container.Bind<RoomObject>()).FromInstance(roomObjectPrefab)).WhenInjectedInto<RoomObjectFactory>();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<CharacterObject>)(object)((MonoInstallerBase)this).Container.Bind<CharacterObject>()).FromInstance(spineObjectPrefab)).WhenInjectedInto<SpineObjectFactory>();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<AnimatedRoomObject>)(object)((MonoInstallerBase)this).Container.Bind<AnimatedRoomObject>()).FromInstance(animatedRoomObjectPrefab)).WhenInjectedInto<AnimatedRoomObjectFactory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<RoomFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<RoomObjectFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<SpineObjectFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<AnimatedRoomObjectFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<RoomObjectFactoryAggregator>()).AsCached();
	}
}
