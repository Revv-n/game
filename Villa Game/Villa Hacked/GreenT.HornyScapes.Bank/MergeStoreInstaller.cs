using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeStore;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class MergeStoreInstaller : Installer<MergeStoreInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindIFactory<CreateDataManager>()).AsSingle();
		((InstallerBase)this).Container.BindInterfacesAndSelfTo<DataManagerCluster>().FromFactory<DataManagerCluster, ClusterFactory<CreateDataManager, DataManagerCluster>>().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MergeStoreConfigInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MergeStoreMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CreateDataFactory>()).AsSingle();
	}
}
