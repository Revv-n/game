using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class MonoViewFactory : MonoBehaviour
{
	[SerializeField]
	private List<MonoView> viewPrefabs;

	private DiContainer container;

	[field: SerializeField]
	public Transform ViewsContainer { get; private set; }

	[Inject]
	public void Init(DiContainer container)
	{
		this.container = container;
	}

	public TView Create<TView>() where TView : MonoView
	{
		TView prefab = (TView)viewPrefabs.Single((MonoView _prefab) => _prefab is TView);
		return container.InstantiatePrefabForComponent<TView>(prefab, ViewsContainer);
	}
}
public class MonoViewFactory<TView> : MonoBehaviourFactory<TView> where TView : MonoBehaviour, IView
{
}
