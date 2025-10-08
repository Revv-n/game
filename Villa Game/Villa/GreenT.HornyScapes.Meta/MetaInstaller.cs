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
		base.Container.BindInterfacesAndSelfTo<RoomConfigController>().AsSingle();
		base.Container.Bind<RoomManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RoomObjectsBuilder>().AsSingle();
	}

	private void BindBundleLoaders()
	{
		base.Container.BindInterfacesTo<IntrinsicLoader>().AsSingle();
		base.Container.BindInterfacesTo<RoomConfigLoader>().AsSingle();
		base.Container.Bind<IBundlesLoader<int, RoomConfig>>().To<RoomConfigProxyLoader>().AsSingle();
		base.Container.Bind<IBundlesLoader<int, SkeletonAnimation>>().To<CharacterSkinLoader>().AsSingle();
	}

	private void BindBackgroundLoad()
	{
		BundleType param = BundleType.StemMap;
		BundleType param2 = BundleType.StemPreloadedMap;
		base.Container.BindInterfacesTo<BackgroundSpritesLoader>().AsCached().WithArguments(param);
		base.Container.Bind<ILoader<IEnumerable<Sprite>>>().WithId("Preload").To<BackgroundSpritesLoader>()
			.AsCached()
			.WithArguments(param2);
		base.Container.BindInterfacesAndSelfTo<BackgroundSpritesCollection>().AsSingle();
	}

	private void BindRoomsLoad()
	{
		base.Container.BindInterfacesTo<HouseConfigLoader>().AsSingle();
		base.Container.BindIFactory<RoomDataConfig, RoomData>().FromFactory<RoomDataFactory>();
		base.Container.BindInterfacesTo<RoomDataLoader>().AsSingle();
		base.Container.Bind<RoomDataManager>().FromNew().AsSingle();
		base.Container.Bind<RoomConfigManager>().FromNew().AsSingle();
		base.Container.Bind<Wrapper<IntrinsicRoomConfig>>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.Bind<RoomObject>().FromInstance(roomObjectPrefab).WhenInjectedInto<RoomObjectFactory>();
		base.Container.Bind<CharacterObject>().FromInstance(spineObjectPrefab).WhenInjectedInto<SpineObjectFactory>();
		base.Container.Bind<AnimatedRoomObject>().FromInstance(animatedRoomObjectPrefab).WhenInjectedInto<AnimatedRoomObjectFactory>();
		base.Container.BindInterfacesTo<RoomFactory>().AsCached();
		base.Container.BindInterfacesTo<RoomObjectFactory>().AsCached();
		base.Container.BindInterfacesTo<SpineObjectFactory>().AsCached();
		base.Container.BindInterfacesTo<AnimatedRoomObjectFactory>().AsSingle();
		base.Container.BindInterfacesTo<RoomObjectFactoryAggregator>().AsCached();
	}
}
