using GreenT.HornyScapes.Settings.UI;
using GreenT.Localizations.Data;
using GreenT.Localizations.Settings;
using GreenT.Settings;
using Zenject;

namespace GreenT.Localizations;

public class LocalizationInstaller : Installer<LocalizationInstaller>
{
	public override void InstallBindings()
	{
		Installer<LocalizationSelectorInstaller>.Install(base.Container);
		Installer<LocalizationVariantsInstaller>.Install(base.Container);
		base.Container.Bind<ILocalizationUrlResolver>().FromResolveGetter((IProjectSettings _settings) => _settings.LocalizationUrlResolver).AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizationLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizationProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizationState>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizationService>().AsSingle();
		base.Container.Bind<LocalizationShortCuts>().AsSingle();
		base.Container.Bind<LanguageSettingsLoadingScreen>().FromResolveGetter((LocalizationScreenFactory factory) => factory.Create()).AsSingle()
			.NonLazy();
	}
}
