using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventGirlCollectionInstaller : MonoInstaller<EventGirlCollectionInstaller>
{
	public EventGirlCardView viewPrefab;

	public Transform CardContainer;

	public override void InstallBindings()
	{
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventGirlCardView.Manager>()).FromNewComponentOn(CardContainer.gameObject).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<EventGirlCardView>()).FromComponentInNewPrefab((Object)viewPrefab)).UnderTransform(CardContainer).AsCached();
	}
}
