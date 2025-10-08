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
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<IView<ICard>>()).FromComponentInNewPrefab((Object)viewPrefab)).UnderTransform(container);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CardViewManager>()).FromNewComponentOn(container.gameObject).AsSingle();
	}
}
