using GreenT.AssetBundles;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.StarShop.Data;
using GreenT.HornyScapes.StarShop.Story;
using GreenT.HornyScapes.StarShop.SubSystems;
using GreenT.HornyScapes.Сrutch;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.StarShop;

public class StarShopInstaller : Installer<StarShopInstaller>
{
	public class StarStorySpriteLoader : AssetBundleLoaderByType<int, Sprite>
	{
		public StarStorySpriteLoader(IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
			: base(assetBundlesLoader, projectSettings, BundleType.StarArt)
		{
		}
	}

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<StarShopStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<StarShopMapper>>().AsSingle();
		base.Container.BindInterfacesTo<StarShopFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<StarShopManager>().FromNew().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StarShopController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StarResetUponProgress>().AsSingle();
		BindFlow();
		BindArtBundleLoader();
	}

	private void BindFlow()
	{
		base.Container.BindInterfacesAndSelfTo<StarShopSubscriptions>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StarShopBoardFlow>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StarShopGirlFlow>().AsSingle();
	}

	private void BindArtBundleLoader()
	{
		base.Container.BindInterfacesAndSelfTo<StarShopArtStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<StarShopArtMapper>>().AsSingle();
		base.Container.BindInterfacesTo<StarShopArtFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<StarShopArtManager>().AsSingle();
		base.Container.Bind<ILoader<int, Sprite>>().To<StarStorySpriteLoader>().AsSingle()
			.WhenInjectedInto<ArtLoadController>();
	}
}
