using System;
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
		Installer<LocalizationSelectorInstaller>.Install(((InstallerBase)this).Container);
		Installer<LocalizationVariantsInstaller>.Install(((InstallerBase)this).Container);
		((FromBinderGeneric<ILocalizationUrlResolver>)(object)((InstallerBase)this).Container.Bind<ILocalizationUrlResolver>()).FromResolveGetter<IProjectSettings>((Func<IProjectSettings, ILocalizationUrlResolver>)((IProjectSettings _settings) => _settings.LocalizationUrlResolver)).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LocalizationLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LocalizationProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LocalizationState>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LocalizationService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<LocalizationShortCuts>()).AsSingle();
		((NonLazyBinder)((FromBinderGeneric<LanguageSettingsLoadingScreen>)(object)((InstallerBase)this).Container.Bind<LanguageSettingsLoadingScreen>()).FromResolveGetter<LocalizationScreenFactory>((Func<LocalizationScreenFactory, LanguageSettingsLoadingScreen>)((LocalizationScreenFactory factory) => factory.Create())).AsSingle()).NonLazy();
	}
}
