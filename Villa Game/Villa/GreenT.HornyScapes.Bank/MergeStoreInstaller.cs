using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeStore;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class MergeStoreInstaller : Installer<MergeStoreInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindIFactory<CreateDataManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DataManagerCluster>().FromFactory<DataManagerCluster, ClusterFactory<CreateDataManager, DataManagerCluster>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MergeStoreConfigInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MergeStoreMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CreateDataFactory>().AsSingle();
	}
}
