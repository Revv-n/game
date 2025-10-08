using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MergePointsInstaller : MonoInstaller<MergeSceneInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MergePointsController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MergePointsIconService>()).AsSingle();
	}
}
