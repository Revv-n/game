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
		base.Container.BindInterfacesAndSelfTo<CheatStarter>().AsSingle();
	}
}
