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
		base.Container.BindInterfacesTo<UIManager>().FromInstance(uiManager);
		base.Container.Bind<MainScreenIndicator>().FromNewComponentOn(uiManager.gameObject).AsSingle()
			.WithArguments(MetaPreset);
		base.Container.Bind<MergeScreenIndicator>().FromNewComponentOn(uiManager.gameObject).AsSingle()
			.WithArguments(CorePreset);
		base.Container.Bind<MainContentScreenIndicator>().FromNewComponentOn(uiManager.gameObject).AsSingle()
			.WithArguments(MainContentPreset);
		base.Container.Bind<SettingsPopupOpener>().AsSingle().WithArguments(SettingsPreset);
		base.Container.BindInterfacesAndSelfTo<IndicatorDisplayService>().AsSingle();
		base.Container.Bind<GirlPromoWindowWrapper>().AsSingle();
	}
}
