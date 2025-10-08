using Zenject;

namespace GreenT.HornyScapes.Stories;

public class StoryInstaller : Installer<StoryInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<StoryController>().AsSingle();
	}
}
