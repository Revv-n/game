using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatInstaller : Installer<CheatInstaller>
{
	public override void InstallBindings()
	{
		AddCheat();
	}

	private void AddCheat()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CheatStarter>()).AsSingle();
	}
}
