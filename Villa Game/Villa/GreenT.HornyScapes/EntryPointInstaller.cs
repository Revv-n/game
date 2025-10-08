using GreenT.HornyScapes.Windows;
using Zenject;

namespace GreenT.HornyScapes;

public class EntryPointInstaller : MonoInstaller<EntryPointInstaller>
{
	public override void InstallBindings()
	{
		BindEntryPoint();
	}

	private void BindEntryPoint()
	{
		base.Container.BindInterfacesTo<EntryPoint>().AsSingle();
	}
}
