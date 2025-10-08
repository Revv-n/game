using GreenT.Data;
using GreenT.HornyScapes.Events.Content;
using Zenject;

namespace GreenT.HornyScapes;

public class ContentStorageInstaller : Installer<ContentStorageInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<ContentStorageProvider>().AsSingle().OnInstantiated(delegate(InjectContext _context, ContentStorageProvider contentStorage)
		{
			_context.Container.Resolve<ISaver>().Add(contentStorage);
		});
		base.Container.BindInterfacesAndSelfTo<ContentStorageController>().AsSingle().OnInstantiated<ContentStorageController>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
	}
}
