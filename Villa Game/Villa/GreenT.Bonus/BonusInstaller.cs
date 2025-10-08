using Zenject;

namespace GreenT.Bonus;

public class BonusInstaller : Installer<BonusInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<BonusController>().AsSingle().NonLazy();
		base.Container.BindInterfacesAndSelfTo<BonusFactory>().AsSingle();
		base.Container.Bind<BonusManager>().AsSingle();
	}
}
