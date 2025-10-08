using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class GameStarterInstaller : MonoInstaller
{
	[SerializeField]
	private WindowOpener windowOpener;

	public override void InstallBindings()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GameStarter>()).AsSingle()).WithArguments<WindowOpener>(windowOpener);
	}
}
