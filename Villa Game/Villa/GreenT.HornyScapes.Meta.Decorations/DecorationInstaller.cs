using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationInstaller : Installer<DecorationInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<DecorationStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<DecorationMapper>>().AsSingle();
		base.Container.BindInterfacesTo<DecorationFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DecorationManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DecorationController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DecorationSubscriptions>().AsSingle();
	}
}
