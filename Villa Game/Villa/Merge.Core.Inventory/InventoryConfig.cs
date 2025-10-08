using UnityEngine;

namespace Merge.Core.Inventory;

[CreateAssetMenu(fileName = "InventoryConfig", menuName = "DL/Configs/Controllers/Inventory")]
public class InventoryConfig : ScriptableObject
{
	[SerializeField]
	private int startOpenOpenSlotsCount;

	[SerializeField]
	private int closedSlotsCount;

	[SerializeField]
	private int firstPrice;

	[SerializeField]
	private float priceMul;

	public int StartOpenSlotsCount => startOpenOpenSlotsCount;

	public int ClosedSlotsCount => closedSlotsCount;

	public int GetSlotUnlockPrice(int slot)
	{
		float num = firstPrice;
		for (int i = startOpenOpenSlotsCount; i < slot; i++)
		{
			num = Mathf.Floor(num * priceMul);
		}
		return Mathf.FloorToInt(num);
	}
}
