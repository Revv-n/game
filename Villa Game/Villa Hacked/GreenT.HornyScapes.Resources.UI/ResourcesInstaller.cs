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
		((FromBinderGeneric<StarAnimController>)(object)((MonoInstallerBase)this).Container.Bind<StarAnimController>()).FromInstance(starAnimController).AsSingle();
		((FromBinderGeneric<JewelAnimController>)(object)((MonoInstallerBase)this).Container.Bind<JewelAnimController>()).FromInstance(jewelAnimController).AsSingle();
	}
}
