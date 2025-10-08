using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "Booster", menuName = "StripClub/Items/Booster", order = 1)]
public class BoosterItemInfo : TemporalItemInfo
{
	[Tooltip("Order in layer of applying to multiplying formula. The more the latter multiplier will be applied to general formula")]
	[SerializeField]
	private int applyOrder = 1;

	public int ApplyOrder => applyOrder;
}
