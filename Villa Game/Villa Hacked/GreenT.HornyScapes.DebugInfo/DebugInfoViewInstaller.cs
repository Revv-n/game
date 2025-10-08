using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.DebugInfo;

public class DebugInfoViewInstaller : MonoInstaller
{
	[SerializeField]
	private DebugInfoViewFactory _factory;

	public override void InstallBindings()
	{
		((FromBinderGeneric<DebugInfoViewFactory>)(object)((MonoInstallerBase)this).Container.Bind<DebugInfoViewFactory>()).FromInstance(_factory).AsSingle();
	}
}
