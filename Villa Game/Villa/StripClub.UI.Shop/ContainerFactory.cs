using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class ContainerFactory : ViewFactory<LotContainer, ContainerView>
{
	public ContainerFactory(DiContainer diContainer, Transform objectContainer, ContainerView prefab)
		: base(diContainer, objectContainer, prefab)
	{
	}
}
