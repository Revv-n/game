using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class PromocodeInstaller : MonoInstaller
{
	public WindowGroupID promocodePopupID;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PromocodeSettings>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PromocodeService>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PromocodePopupWindowOpener>()).AsSingle()).WithArguments<WindowGroupID>(promocodePopupID);
	}
}
