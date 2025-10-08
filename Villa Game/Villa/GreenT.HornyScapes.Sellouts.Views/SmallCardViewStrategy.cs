using GreenT.UI;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class SmallCardViewStrategy : ISmallCardViewStrategy
{
	public void UpdateConstraint(FlexibleGridLayoutGroup layoutGroup)
	{
		layoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
	}
}
