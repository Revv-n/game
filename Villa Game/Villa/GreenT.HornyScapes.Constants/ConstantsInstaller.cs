using GreenT.HornyScapes.Constants.Data;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Constants;

public class ConstantsInstaller : Installer<ConstantsInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesTo<ConstantsDistributor>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ConstantsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<ConstantMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<Constants>().AsSingle();
	}
}
