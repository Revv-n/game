using Zenject;

namespace GreenT.Localizations;

public class LocalizationSelectorInstaller : Installer<LocalizationSelectorInstaller>
{
	public override void InstallBindings()
	{
		BindLanguageSelector<LanguageSelector>();
	}

	private void BindLanguageSelector<T>() where T : LanguageSelector
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<LanguageSelector>)(object)((InstallerBase)this).Container.Bind<LanguageSelector>()).To<T>()).AsSingle();
	}
}
