using Zenject;

namespace GreenT.HornyScapes;

public class InputBlockInstaller : Installer<InputBlockInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesTo<InputBlockController>().AsSingle();
	}
}
