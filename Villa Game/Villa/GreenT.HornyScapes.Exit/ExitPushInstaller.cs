using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes.Exit;

public class ExitPushInstaller : MonoInstaller
{
	public WindowGroupID ExitPreset;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<ExitPushController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ExitPopupOpener>().AsSingle().WithArguments(ExitPreset);
	}
}
