using GreenT.HornyScapes.External.StripClub._Scripts.MiniEvents;
using GreenT.HornyScapes.MiniEvents;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class DataCleanerInstaller : Installer<DataCleanerInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<DataCleanerManager>().AsCached().NonLazy();
		BindInterfacesAndSelfTo<EventMergeShopCleaner>();
		BindInterfacesAndSelfTo<CharacterDataCleaner>();
		BindInterfacesAndSelfTo<BankDataCleaner>();
		BindInterfacesAndSelfTo<StoryDataCleaner>();
		BindInterfacesAndSelfTo<TaskDataCleaner>();
		BindInterfacesAndSelfTo<EventOfferCleaner>();
		base.Container.BindInterfacesAndSelfTo<EventDataCleaner>().AsCached().NonLazy();
		base.Container.BindInterfacesAndSelfTo<FieldCleaner>().FromNew().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassDataCleaner>().AsCached().NonLazy();
		base.Container.BindInterfacesAndSelfTo<MiniEventDataCleaner>().AsCached().NonLazy();
		base.Container.Bind<MiniEventFieldCleaner>().FromNew().AsSingle();
	}

	private void BindInterfacesAndSelfTo<T>() where T : IDataCleaner
	{
		base.Container.BindInterfacesAndSelfTo<T>().AsSingle().OnInstantiated(delegate(InjectContext _, T obj)
		{
			base.Container.Resolve<DataCleanerManager>().Add(obj);
		})
			.NonLazy();
	}
}
