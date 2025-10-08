using System.Collections.Generic;
using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes.Collection;

public class CollectionInstaller : MonoInstaller<CollectionInstaller>
{
	public WindowOpener BackToMeta;

	public WindowOpener BackToCore;

	public WindowOpener BackToMiniEvents;

	public override void InstallBindings()
	{
		Dictionary<ReturnToType, WindowOpener> dictionary = new Dictionary<ReturnToType, WindowOpener>
		{
			[ReturnToType.Meta] = BackToMeta,
			[ReturnToType.Core] = BackToCore,
			[ReturnToType.MiniEvents] = BackToMiniEvents
		};
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<ReturnButtonStrategy>()).AsSingle()).WithArguments<Dictionary<ReturnToType, WindowOpener>>(dictionary);
	}
}
