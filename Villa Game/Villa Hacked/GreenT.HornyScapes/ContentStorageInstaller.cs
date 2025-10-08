using System;
using GreenT.Data;
using GreenT.HornyScapes.Events.Content;
using Zenject;

namespace GreenT.HornyScapes;

public class ContentStorageInstaller : Installer<ContentStorageInstaller>
{
	public override void InstallBindings()
	{
		((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<ContentStorageProvider>()).AsSingle()).OnInstantiated<ContentStorageProvider>((Action<InjectContext, ContentStorageProvider>)delegate(InjectContext _context, ContentStorageProvider contentStorage)
		{
			_context.Container.Resolve<ISaver>().Add(contentStorage);
		});
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<ContentStorageController>()).AsSingle()).OnInstantiated<ContentStorageController>((Action<InjectContext, ContentStorageController>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
	}
}
