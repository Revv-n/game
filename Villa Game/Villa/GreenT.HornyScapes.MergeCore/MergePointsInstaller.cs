using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MergePointsInstaller : MonoInstaller<MergeSceneInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<MergePointsController>().AsSingle();
		base.Container.Bind<MergePointsIconService>().AsSingle();
	}
}
