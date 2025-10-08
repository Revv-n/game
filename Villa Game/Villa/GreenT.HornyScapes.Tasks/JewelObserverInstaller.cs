using GreenT.HornyScapes.Cheats;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class JewelObserverInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		base.Container.Bind<JewelsObserverTemp>().AsSingle().NonLazy();
	}
}
