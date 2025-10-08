using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.MergeCore;
using Merge;
using Merge.Core.Inventory;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
	[SerializeField]
	private LightningSystem grayScaleSystem;

	[SerializeField]
	private GameItem prefab;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GameItemFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GameItemDistributor>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GameItemConverter>()).AsSingle();
		((ConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInstance<LightningSystem>(grayScaleSystem)).WhenInjectedInto<GameItemFactory>();
		((ConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInstance<GameItem>(prefab)).WhenInjectedInto<GameItemFactory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<InventorySettingsProvider>()).AsSingle();
	}
}
