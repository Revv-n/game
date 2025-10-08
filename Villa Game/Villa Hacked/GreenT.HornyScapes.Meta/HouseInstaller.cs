using System;
using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class HouseInstaller : MonoInstaller<HouseInstaller>
{
	[SerializeField]
	private HouseView houseView;

	[SerializeField]
	private HouseNavigationController navigationController;

	[SerializeField]
	private HouseBackgroundBuilder backgroundBuilder;

	public override void InstallBindings()
	{
		((FromBinderGeneric<HouseView>)(object)((MonoInstallerBase)this).Container.Bind<HouseView>()).FromInstance(houseView).AsSingle();
		((InstantiateCallbackConditionCopyNonLazyBinder)((FromBinderGeneric<HouseNavigationController>)(object)((MonoInstallerBase)this).Container.Bind<HouseNavigationController>()).FromInstance(navigationController).AsSingle()).OnInstantiated<HouseNavigationController>((Action<InjectContext, HouseNavigationController>)SetupStarFlow);
		((FromBinderGeneric<HouseBackgroundBuilder>)(object)((MonoInstallerBase)this).Container.Bind<HouseBackgroundBuilder>()).FromInstance(backgroundBuilder).AsSingle();
	}

	public void SetupStarFlow(InjectContext context, HouseNavigationController controller)
	{
		context.Container.Resolve<StartFlow>().Set(controller);
	}
}
