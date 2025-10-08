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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<StarShopMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<StarShopFactory>()).AsCached();
		((FromBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopManager>()).FromNew().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarResetUponProgress>()).AsSingle();
		BindFlow();
		BindArtBundleLoader();
	}

	private void BindFlow()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopSubscriptions>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopBoardFlow>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopGirlFlow>()).AsSingle();
	}

	private void BindArtBundleLoader()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopArtStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<StarShopArtMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<StarShopArtFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopArtManager>()).AsSingle();
		((ConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<ILoader<int, Sprite>>)(object)((InstallerBase)this).Container.Bind<ILoader<int, Sprite>>()).To<StarStorySpriteLoader>()).AsSingle()).WhenInjectedInto<ArtLoadController>();
	}
}
