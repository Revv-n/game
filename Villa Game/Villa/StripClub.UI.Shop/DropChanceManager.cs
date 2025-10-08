using UnityEngine;

namespace StripClub.UI.Shop;

public class DropChanceManager : ViewManagerBase<DropChanceView>
{
	[SerializeField]
	private DropChanceView viewPrefab;

	[SerializeField]
	private Transform container;

	protected override DropChanceView Create()
	{
		return Object.Instantiate(viewPrefab, container);
	}
}
