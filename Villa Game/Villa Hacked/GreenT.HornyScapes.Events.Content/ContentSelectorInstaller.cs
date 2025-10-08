using Zenject;

namespace GreenT.HornyScapes.Events.Content;

public class ContentSelectorInstaller : Installer<ContentSelectorInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<ContentSelectorGroup>()).AsSingle();
	}

	public static void AddSelectorToContainer(InjectContext context, IContentSelector selector)
	{
		context.Container.Resolve<ContentSelectorGroup>().Add(selector);
	}
}
