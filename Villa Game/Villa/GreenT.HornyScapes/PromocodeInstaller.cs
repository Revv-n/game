using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class PromocodeInstaller : MonoInstaller
{
	public WindowGroupID promocodePopupID;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<PromocodeSettings>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PromocodeService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PromocodePopupWindowOpener>().AsSingle().WithArguments(promocodePopupID);
	}
}
