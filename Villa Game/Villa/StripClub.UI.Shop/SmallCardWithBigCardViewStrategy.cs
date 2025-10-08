using GreenT.UI;
using UnityEngine.UI;

namespace StripClub.UI.Shop;

public sealed class SmallCardWithBigCardViewStrategy : ISmallCardViewStrategy
{
	private const int MinDropsCount = 2;

	public void UpdateConstraint(FlexibleGridLayoutGroup layoutGroup, int dropsCount)
	{
		layoutGroup.constraint = ((2 < dropsCount) ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.Flexible);
	}
}
