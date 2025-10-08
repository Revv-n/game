using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventGirlCollectionInstaller : MonoInstaller<EventGirlCollectionInstaller>
{
	public EventGirlCardView viewPrefab;

	public Transform CardContainer;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<EventGirlCardView.Manager>().FromNewComponentOn(CardContainer.gameObject).AsSingle();
		base.Container.BindIFactory<EventGirlCardView>().FromComponentInNewPrefab(viewPrefab).UnderTransform(CardContainer)
			.AsCached();
	}
}
