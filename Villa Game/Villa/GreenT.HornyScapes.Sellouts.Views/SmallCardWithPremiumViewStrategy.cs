using GreenT.UI;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class SmallCardWithPremiumViewStrategy : ISmallCardViewStrategy
{
	public void UpdateConstraint(FlexibleGridLayoutGroup layoutGroup)
	{
		layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
	}
}
