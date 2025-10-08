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
		Dictionary<ReturnToType, WindowOpener> param = new Dictionary<ReturnToType, WindowOpener>
		{
			[ReturnToType.Meta] = BackToMeta,
			[ReturnToType.Core] = BackToCore,
			[ReturnToType.MiniEvents] = BackToMiniEvents
		};
		base.Container.Bind<ReturnButtonStrategy>().AsSingle().WithArguments(param);
	}
}
