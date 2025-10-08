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
		Assert.IsNotNull(_container);
		this._container = _container;
	}

	public LoadingProgressBar Create()
	{
		LoadingProgressBar prefab = defaultLoadingScreenPrefab;
		DiContainer diContainer = _container.ParentContainers.FirstOrDefault((DiContainer _parentContainer) => _parentContainer.HasBinding(typeof(LoadingProgressBar)));
		LoadingProgressBar loadingProgressBar;
		if (diContainer != null)
		{
			loadingProgressBar = diContainer.Resolve<LoadingProgressBar>();
		}
		else
		{
			loadingProgressBar = _container.InstantiatePrefabForComponent<LoadingProgressBar>(prefab, container);
			loadingProgressBar.Initialize(FadeAnimation);
		}
		return loadingProgressBar;
	}
}
