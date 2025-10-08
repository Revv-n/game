using Merge;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

public class SoundInstaller : MonoInstaller<SoundInstaller>
{
	public SoundController SoundController;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SoundController>().FromInstance((object)SoundController).AsSingle();
	}
}
