using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.Localizations.Data;

public class LocalizationVariantsInstaller : Installer<LocalizationVariantsInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<LocalizationVariantsProvider>().AsSingle();
		base.Container.Bind<LocalizationVariantsFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LocalizationVartiantsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<LocalizationVariantMapper>>().AsSingle();
	}
}
