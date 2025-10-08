using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.StarShop.UI;

public class StarShopViewFactory : MonoBehaviour, IFactory<StarShopView>, IFactory
{
	[SerializeField]
	private StarShopView prefab;

	[SerializeField]
	private Transform container;

	private DiContainer diContainer;

	[Inject]
	private void Init(DiContainer diContainer)
	{
		this.diContainer = diContainer;
	}

	public StarShopView Create()
	{
		return diContainer.InstantiatePrefabForComponent<StarShopView>(prefab, container);
	}
}
