using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes.Settings;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.UI.Rewards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class UIInstaller : MonoInstaller<UIInstaller>
{
	[SerializeField]
	private UIManager uiManager;

	public WindowGroupID MetaPreset;

	public WindowGroupID CorePreset;

	public WindowGroupID MainContentPreset;

	public WindowGroupID SettingsPreset;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesTo<UIManager>().FromInstance((object)uiManager);
		((ArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<MainScreenIndicator>()).FromNewComponentOn(uiManager.gameObject).AsSingle()).WithArguments<WindowGroupID>(MetaPreset);
		((ArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<MergeScreenIndicator>()).FromNewComponentOn(uiManager.gameObject).AsSingle()).WithArguments<WindowGroupID>(CorePreset);
		((ArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<MainContentScreenIndicator>()).FromNewComponentOn(uiManager.gameObject).AsSingle()).WithArguments<WindowGroupID>(MainContentPreset);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SettingsPopupOpener>()).AsSingle()).WithArguments<WindowGroupID>(SettingsPreset);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<IndicatorDisplayService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GirlPromoWindowWrapper>()).AsSingle();
	}
}
