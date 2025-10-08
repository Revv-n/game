using GreenT.HornyScapes.Booster.Effect;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Booster;

public class BoosterInstaller : Installer<BoosterInstaller>
{
	public override void InstallBindings()
	{
		BindCore();
		BindEffect();
	}

	private void BindCore()
	{
		base.Container.BindInterfacesAndSelfTo<BoosterService>().AsSingle();
		base.Container.Bind<BoosterModelFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BoosterStorage>().AsSingle();
		base.Container.BindInterfacesTo<BoosterStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BoosterMapperManager>().AsSingle();
		base.Container.Bind<StructureInitializerProxyWithArrayFromConfig<BoosterMapper>>().AsSingle();
	}

	private void BindEffect()
	{
		base.Container.BindInterfacesTo<BoosterBonusFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EnergyBonusService>().AsSingle();
		base.Container.BindInterfacesTo<SummonBonusService>().AsSingle();
	}
}
