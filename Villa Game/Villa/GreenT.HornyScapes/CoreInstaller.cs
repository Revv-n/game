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
		base.Container.BindInterfacesAndSelfTo<GameItemFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GameItemDistributor>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GameItemConverter>().AsSingle();
		base.Container.BindInstance(grayScaleSystem).WhenInjectedInto<GameItemFactory>();
		base.Container.BindInstance(prefab).WhenInjectedInto<GameItemFactory>();
		base.Container.Bind<InventorySettingsProvider>().AsSingle();
	}
}
