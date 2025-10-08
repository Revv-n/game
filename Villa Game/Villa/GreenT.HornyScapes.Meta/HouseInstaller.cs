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
		base.Container.Bind<HouseView>().FromInstance(houseView).AsSingle();
		base.Container.Bind<HouseNavigationController>().FromInstance(navigationController).AsSingle()
			.OnInstantiated<HouseNavigationController>(SetupStarFlow);
		base.Container.Bind<HouseBackgroundBuilder>().FromInstance(backgroundBuilder).AsSingle();
	}

	public void SetupStarFlow(InjectContext context, HouseNavigationController controller)
	{
		context.Container.Resolve<StartFlow>().Set(controller);
	}
}
