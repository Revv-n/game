using Merge;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

public class SoundInstaller : MonoInstaller<SoundInstaller>
{
	public SoundController SoundController;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<SoundController>().FromInstance(SoundController).AsSingle();
	}
}
