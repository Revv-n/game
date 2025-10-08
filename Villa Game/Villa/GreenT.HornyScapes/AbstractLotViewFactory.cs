using StripClub.Model.Shop.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class AbstractLotViewFactory<TContainer, TView> : MonoBehaviour, IFactory<TContainer, TView>, IFactory
{
	[SerializeField]
	protected Transform viewContainer;

	protected BundlesProviderBase bundlesProvider;

	protected DiContainer container;

	[Inject]
	public void Init(DiContainer container, BundlesProviderBase bundlesProvider)
	{
		this.bundlesProvider = bundlesProvider;
		this.container = container;
	}

	public abstract TView Create(TContainer lotLotContainer);

	protected T TryGetView<T>(ContentSource contentSource, string viewName)
	{
		return bundlesProvider.TryFindInConcreteBundle<GameObject>(contentSource, viewName).GetComponent<T>();
	}
}
