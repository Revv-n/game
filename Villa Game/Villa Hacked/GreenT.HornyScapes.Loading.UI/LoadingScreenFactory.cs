using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Loading.UI;

public class LoadingScreenFactory : MonoBehaviour, IFactory<LoadingProgressBar>, IFactory
{
	[SerializeField]
	private LoadingProgressBar defaultLoadingScreenPrefab;

	[SerializeField]
	private LoadingProgressBar haremLoadingScreenPrefab;

	[SerializeField]
	private Transform container;

	[SerializeField]
	private bool FadeAnimation;

	private DiContainer _container;

	[Inject]
	public void Init(DiContainer _container)
	{
		Assert.IsNotNull((object)_container);
		this._container = _container;
	}

	public LoadingProgressBar Create()
	{
		LoadingProgressBar loadingProgressBar = defaultLoadingScreenPrefab;
		DiContainer val = _container.ParentContainers.FirstOrDefault((DiContainer _parentContainer) => _parentContainer.HasBinding(typeof(LoadingProgressBar)));
		LoadingProgressBar loadingProgressBar2;
		if (val != null)
		{
			loadingProgressBar2 = val.Resolve<LoadingProgressBar>();
		}
		else
		{
			loadingProgressBar2 = _container.InstantiatePrefabForComponent<LoadingProgressBar>((Object)loadingProgressBar, container);
			loadingProgressBar2.Initialize(FadeAnimation);
		}
		return loadingProgressBar2;
	}
}
