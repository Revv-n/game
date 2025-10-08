using Zenject;

namespace GreenT.HornyScapes.Stories;

public class StoryInstaller : Installer<StoryInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StoryController>()).AsSingle();
	}
}
