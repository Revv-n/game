using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class MonoBehaviourFactory<TView> : MonoBehaviour, IFactory<TView>, IFactory where TView : MonoBehaviour
{
	[SerializeField]
	private TView prefab;

	[Inject]
	protected readonly DiContainer _container;

	public virtual TView Create()
	{
		if (prefab == null)
		{
			Debug.Log("Null prefab given to factory create method when instantiating object.", this);
		}
		return _container.InstantiatePrefabForComponent<TView>(prefab);
	}
}
