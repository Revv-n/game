using Zenject;

namespace GreenT.HornyScapes;

public sealed class LastChanceCommonInstaller : Installer<LastChanceCommonInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<LastChanceManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LastChanceFactory>().AsSingle();
	}
}
