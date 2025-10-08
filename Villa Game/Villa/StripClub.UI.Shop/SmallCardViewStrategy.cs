using GreenT.UI;
using UnityEngine.UI;

namespace StripClub.UI.Shop;

public sealed class SmallCardViewStrategy : ISmallCardViewStrategy
{
	private const int MinDropsCount = 3;

	public void UpdateConstraint(FlexibleGridLayoutGroup layoutGroup, int dropsCount)
	{
		layoutGroup.constraint = ((3 < dropsCount) ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.Flexible);
	}
}
