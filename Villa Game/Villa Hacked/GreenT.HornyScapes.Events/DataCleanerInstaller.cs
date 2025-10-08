using System;
using GreenT.HornyScapes.External.StripClub._Scripts.MiniEvents;
using GreenT.HornyScapes.MiniEvents;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class DataCleanerInstaller : Installer<DataCleanerInstaller>
{
	public override void InstallBindings()
	{
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DataCleanerManager>()).AsCached()).NonLazy();
		BindInterfacesAndSelfTo<EventMergeShopCleaner>();
		BindInterfacesAndSelfTo<CharacterDataCleaner>();
		BindInterfacesAndSelfTo<BankDataCleaner>();
		BindInterfacesAndSelfTo<StoryDataCleaner>();
		BindInterfacesAndSelfTo<TaskDataCleaner>();
		BindInterfacesAndSelfTo<EventOfferCleaner>();
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<EventDataCleaner>()).AsCached()).NonLazy();
		((FromBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<FieldCleaner>()).FromNew().AsSingle();
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassDataCleaner>()).AsCached()).NonLazy();
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventDataCleaner>()).AsCached()).NonLazy();
		((FromBinder)((InstallerBase)this).Container.Bind<MiniEventFieldCleaner>()).FromNew().AsSingle();
	}

	private void BindInterfacesAndSelfTo<T>() where T : IDataCleaner
	{
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<T>()).AsSingle()).OnInstantiated<T>((Action<InjectContext, T>)delegate(InjectContext _, T obj)
		{
			((InstallerBase)this).Container.Resolve<DataCleanerManager>().Add(obj);
		})).NonLazy();
	}
}
