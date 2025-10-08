using GreenT.HornyScapes.Events;
using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.Collection;

public class CollectionWindowInstaller : MonoInstaller
{
	[SerializeField]
	private EventGirlCardView viewPrefab;

	[SerializeField]
	private Transform container;

	public override void InstallBindings()
	{
		base.Container.BindIFactory<IView<ICard>>().FromComponentInNewPrefab(viewPrefab).UnderTransform(container);
		base.Container.BindInterfacesAndSelfTo<CardViewManager>().FromNewComponentOn(container.gameObject).AsSingle();
	}
}
