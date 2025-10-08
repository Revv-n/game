using GreenT.HornyScapes.Animations;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Resources.UI;

public class ResourcesInstaller : MonoInstaller<ResourcesInstaller>
{
	[SerializeField]
	private StarAnimController starAnimController;

	[SerializeField]
	private JewelAnimController jewelAnimController;

	public override void InstallBindings()
	{
		base.Container.Bind<StarAnimController>().FromInstance(starAnimController).AsSingle();
		base.Container.Bind<JewelAnimController>().FromInstance(jewelAnimController).AsSingle();
	}
}
